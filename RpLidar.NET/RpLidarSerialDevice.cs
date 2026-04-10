using RpLidar.NET.Entities;
using RpLidar.NET.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace RpLidar.NET
{
    // Local stub — replace with RpLidar.NET.Entities.HqScanFlags after WU-02 merges
    [Flags]
    public enum HqScanFlags : ushort { None = 0, Ccw = 1, RawEncoder = 2, RawDistance = 4 }

    /// <summary>
    /// RPLidar A2, A3
    /// </summary>
    public class RpLidarSerialDevice : ILidarService
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RpLidarSerialDevice"/> and configures the serial port.
        /// Call <see cref="Start"/> or <see cref="Connect"/> to open the connection.
        /// </summary>
        /// <param name="settings">Device configuration settings.</param>
        public RpLidarSerialDevice(ILidarSettings settings)
        {
            _settings = settings;
            _timeout = 1000;
            CreateSerial(settings.Port);
        }

        private readonly ILidarSettings _settings;
        private readonly int _timeout;

        /// <summary>
        /// Serial Port Connection
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        /// Connection Status
        /// </summary>
        private bool _isConnected;

        /// <summary>
        /// Motor Status
        /// </summary>
        private bool _motorRunning;

        /// <summary>
        /// Scanning Status
        /// </summary>
        private bool _isScanning;

        /// <summary>
        /// Thread was stopped
        /// </summary>
        private bool _isStoppedThread;

        private bool _isStop;

        /// <summary>
        /// Scanning Thread
        /// </summary>
        private Thread _scanThread = null;

        private void ScanProcess(List<LidarPoint> points)
        {
            if (!points.Any())
                return;
            if (!_isScanning)
                return;

            if (LidarPointScanEvent != null)
            {
                LidarPointScanEvent.Invoke(points);
            }
            if (LidarPointGroupScanEvent != null)
            {
                var group = new LidarPointGroup(0, 0);
                group.Settings = _settings;
                group.AddRange(points);
                LidarPointGroupScanEvent.Invoke(group);
            }
        }

        public event LidarPointScanEvenHandler LidarPointScanEvent;

        public event LidarPointGroupScanEvenHandler LidarPointGroupScanEvent;

        private void CreateSerial(string portName)
        {
            // Create a new SerialPort
            _serialPort = new SerialPort();
            _serialPort.PortName = portName;
            _serialPort.BaudRate = _settings.BaudRate;
            //Setup RPLidar specifics
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            // Set the read/write timeouts
            _serialPort.ReadTimeout = _timeout;
            _serialPort.WriteTimeout = _timeout;
        }

        /// <summary>
        /// Opens the serial connection to the RPLidar device.
        /// Does nothing if the device is already connected.
        /// </summary>
        public void Connect()
        {
            if (_isConnected)
            {
                Console.WriteLine("Already connected to RPLidar.");
                return;
            }

            try
            {
                _serialPort.Open();
                _isConnected = true;
                Console.WriteLine($"Connected to RPLidar on {_serialPort.PortName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to RPLidar on {_serialPort.PortName}: {e.Message}");
            }
        }

        /// <summary>
        /// Disconnect Serial connection to RPLIDAR
        /// </summary>
        public void Disconnect()
        {
            if (_serialPort != null)
            {
                //Stop scan
                if (_isScanning)
                {
                    StopScan();
                }
                _isScanning = false;
                _serialPort.Close();
                this._isConnected = false;
            }
        }

        /// <summary>
        /// Dispose Object
        /// </summary>
        public void Dispose()
        {
            try
            {
                StopScan();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, e);
            }

            _motorRunning = false;
            if (_serialPort != null)
            {
                if (_isConnected)
                {
                    Disconnect();
                }

                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        /// <summary>
        /// Send Request to RPLidar
        /// </summary>
        /// <param name="command"></param>
        public IDataResponse SendRequest(Command command)
        {
            if (_isConnected)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.SendRequest(command);

                var sleep = command.GetSleepInterval();
                if (sleep > 0)
                {
                    Thread.Sleep(sleep);
                }

                if (!command.HasResponse())
                    return null;

                var responseDescriptor = ReadResponseDescriptor();

                if (responseDescriptor.RpDataType != RpDataType.Scan)
                {
                    var response = GetDataResponse(responseDescriptor.ResponseLength, responseDescriptor.RpDataType);
                    return response;
                }
            }

            return null;
        }

        /// <summary>
        /// Send Request to RPLidar
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        public IDataResponse SendCommand(byte command, byte[] data = null)
        {
            if (_isConnected)
            {
                //Clear input buffer of any junk
                _serialPort.DiscardInBuffer();

                _serialPort.SendCommand(command, data);

                var hasResponse = CommandHelper.GetHasResponse(command);

                //We must sleep after executing some commands
                var sleep = command.GetMustSleep();
                if (sleep > 0)
                {
                    Thread.Sleep(sleep);
                }

                if (!hasResponse)
                    return null;
            }

            return null;
        }

        private IDataResponse GetDataResponse(int len, RpDataType rpDataType)
        {
            var bytes = Read(len, 1000);
            var response = DataResponseHelper.ToDataResponse(rpDataType, bytes);
            return response;
        }

        private ResponseDescriptor ReadResponseDescriptor()
        {
            var bytes = Read(Constants.DescriptorLength, 1000);

            var descriptor = bytes.ToResponseDescriptor();
            return descriptor;
        }

        /// <summary>
        /// Start RPLidar Motor
        /// </summary>
        public void StartMotor()
        {
            if (_isConnected)
            {
                _serialPort.DtrEnable = false;
                _motorRunning = true;
                var pwm = new RplidarPayloadMotorPwm()
                {
                    //pwm_value = 660
                    pwm_value = _settings.Pwm
                };
                var data = pwm.GetBytes();
                this.SendCommand((byte)Command.StartPwm, data);
            }
        }

        /// <summary>
        /// Stop RPLidar Motor
        /// </summary>
        public void StopMotor()
        {
            if (_isConnected)
            {
                var pwm = new RplidarPayloadMotorPwm()
                {
                    pwm_value = 0
                };
                var data = pwm.GetBytes();
                this.SendCommand((byte)Command.StartPwm, data);
                _serialPort.DtrEnable = true;
                _motorRunning = false;
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Force Start Scanning
        /// Use with care, returns results without motor rotation synchronization
        /// </summary>
        public void ForceScan()
        {
            //Not already scanning
            if (!_isScanning)
            {
                //Have to be connected
                if (_isConnected)
                {
                    _isScanning = true;
                    //Motor must be running
                    if (!_motorRunning)
                        this.StartMotor();
                    //Start Scan
                    this.SendRequest(Command.ForceScan);

                    //Start Scan read thread
                    _scanThread = new Thread(ScanThread);
                    _scanThread.Start();
                }
            }
        }

        /// <summary>
        /// Force Start Scanning
        /// Use with care, returns results without motor rotation synchronization
        /// </summary>
        public void ForceScanExpress()
        {
            //Not already scanning
            if (!_isScanning)
            {
                //Have to be connected
                if (_isConnected)
                {
                    _isScanning = true;
                    //Motor must be running
                    if (!_motorRunning)
                        this.StartMotor();
                    //Start Scan
                    // Classic Capsule mode (Standard working_mode = 0) responds with 0x82;
                    // Boost/Sensitivity modes respond with 0x84 (Ultra-Capsule, handled as 0x81 internally).
                    _activeScanAnsType = _settings.ScanMode == ScanMode.Standard
                        ? (byte)RpDataType.CapsuleScan
                        : (byte)RpDataType.Scan;
                    var scan = new RplidarPayloadExpressScan()
                    {
                        working_mode = (byte)_settings.ScanMode,
                        working_flags = 1,
                    };
                    Console.WriteLine($"Start:{Command.ExpressScan}");
                    var bytes = scan.GetBytes();
                    this.SendCommand((byte)Command.ExpressScan, bytes);
                    Console.WriteLine($"Send:{Command.ExpressScan}");

                    //Start Scan read thread
                    _scanThread = new Thread(ScanThread);
                    _scanThread.Start();
                }
            }
        }

        /// <summary>
        /// Start Scanning
        /// </summary>
        public void StartScan()
        {
            //Not already scanning
            if (!_isScanning)
            {
                //Have to be connected
                if (_isConnected)
                {
                    _isScanning = true;
                    //Motor must be running
                    if (!_motorRunning)
                        this.StartMotor();
                    //Start Scan
                    _activeScanAnsType = (byte)RpDataType.Scan;
                    this.SendRequest(Command.Scan);
                    //Start Scan read thread
                    _scanThread = new Thread(ScanThread);
                    _scanThread.Start();
                }
            }
        }

        /// <summary>
        /// Stop Scanning
        /// </summary>
        public void StopScan()
        {
            if (!_isConnected)
                return;

            while (!_isStoppedThread && _isScanning)
            {
                _isScanning = false;
                Thread.Sleep(10);
            }
            Thread.Sleep(20);
            this.SendCommand((byte)Command.Stop);
            Thread.Sleep(20);
            StopMotor();
        }

        /// <summary>
        /// Thread used for scanning
        /// Populates a list of Measurements, and adds that list to
        /// </summary>
        private void ScanThread()
        {
            var points = new List<LidarPoint>();
            _isStoppedThread = false;
            var sw = new Stopwatch();
            sw.Start();
            while (_isScanning)
            {
                try
                {
                    if (!_serialPort.IsOpen)
                    {
                        _isConnected = false;
                        Connect();
                    }

                    var lidarPoints = WaitAndParseData();

                    foreach (var lidarPoint in lidarPoints)
                    {
                        if (!_isScanning)
                            break;

                        //is new 360 degree scan
                        if (lidarPoint.StartFlag)
                        {
                        }

                        if (lidarPoint.IsValid)
                        {
                            points.Add(lidarPoint);
                        }
                    }

                    if (sw.ElapsedMilliseconds > _settings.ElapsedMilliseconds)
                    {
                        sw.Restart();
                        ScanProcess(points);
                        points = new List<LidarPoint>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            }

            _isStoppedThread = true;
        }

        private byte[] Read(int len, int timeout)
        {
            try
            {
                var data = _serialPort.Read(len, timeout);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                this._isConnected = false;
                this.Disconnect();

                return new byte[0];
            }
        }

        private bool _isPreviousCapsuleDataRdy;
        private RplidarResponseUltraCapsuleMeasurementNodes prev = null;

        private RplidarResponseCapsuleMeasurementNodes _prevCapsule;

        /// <summary>Tracks which express-scan answer type is active.</summary>
        private byte _activeScanAnsType = (byte)RpDataType.Scan;

        private RplidarResponseDenseCapsuleMeasurementNodes _prevDense;
        private RplidarResponseUltraDenseCapsuleMeasurementNodes _prevUltraDense;

        private List<LidarPoint> UltraCapsuleToNormal(RplidarResponseUltraCapsuleMeasurementNodes capsule)
        {
            var result = new List<LidarPoint>();

            {
                if (_isPreviousCapsuleDataRdy)
                {
                    // exported C++ code
                    int diffAngle_q8;
                    int currentStartAngle_q8 = ((capsule.start_angle_sync_q6 & 0x7FFF) << 2);
                    int prevStartAngle_q8 = ((prev.start_angle_sync_q6 & 0x7FFF) << 2);

                    diffAngle_q8 = (currentStartAngle_q8) - (prevStartAngle_q8);
                    if (prevStartAngle_q8 > currentStartAngle_q8)
                    {
                        diffAngle_q8 += (360 << 8);
                    }

                    int angleInc_q16 = (diffAngle_q8 << 3) / 3;
                    int currentAngle_raw_q16 = (prevStartAngle_q8 << 8);
                    //for (var pos = 0; pos < capsule.ultra_cabins); ++pos)
                    for (var pos = 0; pos < capsule.ultra_cabins.Length; pos++)
                    {
                        var ultraCabin = prev.ultra_cabins[pos];
                        var dist_q2 = new int[3];
                        var angle_q6 = new int[3];
                        var syncBit = new int[3];

                        var combined_x3 = ultraCabin;

                        // unpack ...
                        int dist_major = (int)(combined_x3 & 0xFFF);

                        // signed partical integer, using the magic shift here
                        // DO NOT TOUCH

                        int dist_predict1 = (((int)(combined_x3 << 10)) >> 22);
                        int dist_predict2 = (((int)combined_x3) >> 22);

                        int dist_major2;

                        int scalelvl1, scalelvl2;

                        // prefetch next ...
                        if (pos == prev.ultra_cabins.Length - 1)
                        {
                            dist_major2 = (int)(capsule.ultra_cabins[0] & 0xFFF);
                        }
                        else
                        {
                            dist_major2 = (int)(prev.ultra_cabins[pos + 1] & 0xFFF);
                        }

                        // decode with the var bit scale ...
                        dist_major = (int)DataResponseHelper._varbitscale_decode(dist_major, out scalelvl1);
                        dist_major2 = (int)DataResponseHelper._varbitscale_decode(dist_major2, out scalelvl2);

                        int dist_base1 = dist_major;
                        int dist_base2 = dist_major2;

                        //if ((!dist_major) && dist_major2)
                        if ((dist_major == 0) && dist_major2 != 0)
                        {
                            dist_base1 = dist_major2;
                            scalelvl1 = scalelvl2;
                        }

                        dist_q2[0] = (dist_major << 2);
                        if ((dist_predict1 == 0xFFFFFE00) || (dist_predict1 == 0x1FF))
                        {
                            dist_q2[1] = 0;
                        }
                        else
                        {
                            dist_predict1 = (dist_predict1 << scalelvl1);
                            dist_q2[1] = (dist_predict1 + dist_base1) << 2;
                        }

                        if ((dist_predict2 == 0xFFFFFE00) || (dist_predict2 == 0x1FF))
                        {
                            dist_q2[2] = 0;
                        }
                        else
                        {
                            dist_predict2 = (dist_predict2 << scalelvl2);
                            dist_q2[2] = (dist_predict2 + dist_base2) << 2;
                        }

                        for (int cpos = 0; cpos < 3; ++cpos)
                        {
                            syncBit[cpos] = (((currentAngle_raw_q16 + angleInc_q16) % (360 << 16)) < angleInc_q16)
                                ? 1
                                : 0;

                            int offsetAngleMean_q16 = (int)(7.5 * Math.PI * (1 << 16) / 180.0);

                            if (dist_q2[cpos] >= (50 * 4))
                            {
                                int k1 = 98361;
                                int k2 = (int)(k1 / dist_q2[cpos]);

                                offsetAngleMean_q16 = (int)(8 * Math.PI * (1 << 16) / 180) - (k2 << 6) -
                                                      (k2 * k2 * k2) / 98304;
                            }

                            angle_q6[cpos] =
                                ((currentAngle_raw_q16 - (int)(offsetAngleMean_q16 * 180 / Math.PI)) >> 10);
                            currentAngle_raw_q16 += angleInc_q16;

                            if (angle_q6[cpos] < 0) angle_q6[cpos] += (360 << 6);
                            if (angle_q6[cpos] >= (360 << 6)) angle_q6[cpos] -= (360 << 6);

                            var flag = (syncBit[cpos] | (syncBit[cpos] == 0 ? 2 : 0));

                            var quality = dist_q2[cpos] > 0 ? (0x2F << DataResponseHelper.RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT) : 0;
                            var convert = (flag & DataResponseHelper.RpLidarRespMeasurementSyncBit) | ((quality >> DataResponseHelper.RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT) << DataResponseHelper.RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT);

                            var node = new LidarPoint();
                            node.Flag = flag;
                            node.Quality = (ushort)convert;
                            var angle = (ushort)((angle_q6[cpos] << 8) / 90);
                            node.Angle = angle * 90f / 16384f;
                            node.Distance = dist_q2[cpos] / 4;
                            if ((flag & 1) > 0)
                            {
                                node.StartFlag = true;
                            }

                            if (node.Distance > 0 && node.Quality > 0)
                            {
                                if (node.Distance > _settings.MaxDistance) continue;
                                result.Add(node);
                            }
                        }
                    }
                }

                prev = capsule;
                _isPreviousCapsuleDataRdy = true;
            }

            return result;
        }

        private List<LidarPoint> ParseStandardScanPackets(byte[] buffer)
        {
            var result = new List<LidarPoint>();
            for (var offset = 0; offset + Constants.ScanDataResponseLength <= buffer.Length; offset += Constants.ScanDataResponseLength)
            {
                var qualityAndFlags = buffer[offset];
                var checkBit = (qualityAndFlags & 0x02) >> 1;
                if (checkBit != 1)
                    continue;

                var quality = qualityAndFlags >> 2;
                var startFlag = (qualityAndFlags & 0x01) == 1;

                var angleRaw = (ushort)(buffer[offset + 1] | (buffer[offset + 2] << 8));
                var angle = (angleRaw >> 1) / 64.0f;

                var distRaw = (ushort)(buffer[offset + 3] | (buffer[offset + 4] << 8));
                var distance = distRaw / 4.0f;

                if (distance > _settings.MaxDistance)
                    continue;

                var point = new LidarPoint
                {
                    Angle = angle,
                    Distance = distance,
                    Quality = (ushort)quality,
                    StartFlag = startFlag,
                };
                result.Add(point);
            }
            return result;
        }

        /// <summary>
        /// Converts a Classic Capsule packet pair into a list of <see cref="LidarPoint"/> measurements.
        /// Ported from the SLAMTEC C++ SDK <c>_capsuleToNormal()</c>.
        /// </summary>
        private List<LidarPoint> CapsuleToNormal(
            RplidarResponseCapsuleMeasurementNodes current,
            RplidarResponseCapsuleMeasurementNodes previous)
        {
            var result = new List<LidarPoint>();

            float currentStartAngle = (current.StartAngleSyncQ6 & 0x7FFF) * 90.0f / 16384.0f;
            float previousStartAngle = (previous.StartAngleSyncQ6 & 0x7FFF) * 90.0f / 16384.0f;

            float diffAngle = currentStartAngle - previousStartAngle;
            if (diffAngle < 0.0f)
                diffAngle += 360.0f;

            // 16 cabins × 2 samples = 32 samples span the inter-packet angle
            float angleInterval = diffAngle / 32.0f;

            for (int cabin = 0; cabin < 16; cabin++)
            {
                int offset = cabin * 5;

                ushort da1 = (ushort)(previous.CabinData[offset]     | (previous.CabinData[offset + 1] << 8));
                ushort da2 = (ushort)(previous.CabinData[offset + 2] | (previous.CabinData[offset + 3] << 8));
                byte offsets = previous.CabinData[offset + 4];

                // bits [15:3] = 13-bit distance in mm*4; bits [1:0] are unused flags
                int dist1 = (da1 >> 2) & 0x3FFF;
                int dist2 = (da2 >> 2) & 0x3FFF;

                // 4-bit sign-extend each nibble of offsets, then scale to degrees (Q3 → °)
                int raw1 = offsets & 0x0F;
                int raw2 = (offsets >> 4) & 0x0F;
                float delta1 = (raw1 >= 8 ? (raw1 - 16) : raw1) / 8.0f;
                float delta2 = (raw2 >= 8 ? (raw2 - 16) : raw2) / 8.0f;

                float angle1 = ((previousStartAngle + angleInterval * (cabin * 2)       + delta1) % 360.0f + 360.0f) % 360.0f;
                float angle2 = ((previousStartAngle + angleInterval * (cabin * 2 + 1)   + delta2) % 360.0f + 360.0f) % 360.0f;

                if (dist1 > 0)
                    result.Add(new LidarPoint
                    {
                        Angle = angle1,
                        Distance = dist1 / 4.0f,
                        Quality = (ushort)(0x2F << DataResponseHelper.RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT)
                    });

                if (dist2 > 0)
                    result.Add(new LidarPoint
                    {
                        Angle = angle2,
                        Distance = dist2 / 4.0f,
                        Quality = (ushort)(0x2F << DataResponseHelper.RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT)
                    });
            }

            return result;
        }

        /// <summary>
        /// Computes the per-sample angle step between two consecutive capsule start angles,
        /// accounting for wrap-around at 360°. Used by all Dense decoder variants.
        /// </summary>
        private static (float previousAngle, float angleStep) ComputeDenseAngleStep(
            ushort currentStartAngleSyncQ6, ushort previousStartAngleSyncQ6)
        {
            float currentAngle  = (currentStartAngleSyncQ6  & 0x7FFF) * 90.0f / 16384.0f;
            float previousAngle = (previousStartAngleSyncQ6 & 0x7FFF) * 90.0f / 16384.0f;
            float diffAngle = currentAngle - previousAngle;
            if (diffAngle < 0) diffAngle += 360.0f;
            return (previousAngle, diffAngle / 40.0f);
        }

        private List<LidarPoint> DenseCapsuleToNormal(
            RplidarResponseDenseCapsuleMeasurementNodes current,
            RplidarResponseDenseCapsuleMeasurementNodes previous)
        {
            var (previousAngle, angleStep) = ComputeDenseAngleStep(
                current.StartAngleSyncQ6, previous.StartAngleSyncQ6);
            var points = new List<LidarPoint>(40);
            for (int i = 0; i < 40; i++)
            {
                float dist = previous.Distances[i] / 4.0f; // Q2 → mm
                if (dist <= 0 || dist > _settings.MaxDistance) continue;
                points.Add(new LidarPoint
                {
                    Angle = (previousAngle + angleStep * i) % 360.0f,
                    Distance = dist,
                    Quality = 15,
                    StartFlag = i == 0
                });
            }
            return points;
        }

        private List<LidarPoint> UltraDenseCapsuleToNormal(
            RplidarResponseUltraDenseCapsuleMeasurementNodes current,
            RplidarResponseUltraDenseCapsuleMeasurementNodes previous)
        {
            var (previousAngle, angleStep) = ComputeDenseAngleStep(
                current.StartAngleSyncQ6, previous.StartAngleSyncQ6);
            var points = new List<LidarPoint>(40);
            for (int i = 0; i < 40; i++)
            {
                float dist = previous.Distances[i] / 4.0f; // Q2 → mm
                if (dist <= 0 || dist > _settings.MaxDistance) continue;
                points.Add(new LidarPoint
                {
                    Angle = (previousAngle + angleStep * i) % 360.0f,
                    Distance = dist,
                    Quality = previous.Qualities[i],
                    StartFlag = i == 0
                });
            }
            return points;
        }

        /// <summary>
        /// Start HQ scan (FW 1.24+). Sends command 0x83 with RplidarPayloadHqScan payload.
        /// </summary>
        public void StartHqScan(HqScanFlags flags = HqScanFlags.None)
        {
            if (!_isScanning)
            {
                if (_isConnected)
                {
                    _isScanning = true;
                    if (!_motorRunning)
                        this.StartMotor();
                    var payload = new RplidarPayloadHqScan
                    {
                        WorkingMode  = 0,
                        WorkingFlags = (ushort)flags,
                        Param        = 0
                    };
                    var payloadBytes = payload.GetBytes();
                    SendCommand((byte)Command.HqScan, payloadBytes);
                    _scanThread = new Thread(ScanThread);
                    _scanThread.Start();
                }
            }
        }

        private List<LidarPoint> HqScanToNormal(List<RplidarResponseHqMeasurementNode> nodes)
        {
            var points = new List<LidarPoint>(nodes.Count);
            foreach (var node in nodes)
            {
                float dist = node.DistanceMm;
                if (dist <= 0 || dist > _settings.MaxDistance) continue;
                points.Add(new LidarPoint
                {
                    Angle     = node.AngleDegrees,
                    Distance  = dist,
                    Quality   = node.Quality,
                    StartFlag = node.IsNewScan,
                    Flag      = node.Flag
                });
            }
            return points;
        }

        private bool _render;

        private readonly List<byte[]> _prevData = new List<byte[]>(100);

        private List<LidarPoint> WaitAndParseData()
        {
            const int HqNodeSize = 16;
            int bufSize;
            if (_settings.ScanMode == ScanMode.HqScan)
                bufSize = HqNodeSize * 10;
            else if (_activeScanAnsType == (byte)RpDataType.CapsuleScan)
                bufSize = 80 * 3;
            else if (_settings.ScanMode != ScanMode.Standard)
                bufSize = 132 * 3;
            else
                bufSize = 2002;

            byte[] data = null;
            var i = 0;
            while (_serialPort.BytesToRead < bufSize && _isScanning && i < 5)
            {
                Thread.Sleep(5);
                i++;
            }

            if (_serialPort.BytesToRead > 0)
            {
                if (_activeScanAnsType == (byte)RpDataType.CapsuleScan)
                {
                    var bytesToRead = _serialPort.BytesToRead;
                    data = new byte[bytesToRead];
                    _serialPort.Read(data, 0, data.Length);

                    _prevData.Add(data);
                    var aggregates = _prevData.Count == 1 ? _prevData[0] : _prevData.SelectMany(x => x).ToArray();
                    var capsuleQueue = aggregates.WaitCapsuledNode();
                    var points = new List<LidarPoint>();

                    while (capsuleQueue.Count > 0)
                    {
                        var capsule = capsuleQueue.Dequeue();

                        if ((capsule.StartAngleSyncQ6 & 0x8000) != 0)
                            _prevCapsule = null;

                        if (_prevCapsule != null)
                            points.AddRange(CapsuleToNormal(capsule, _prevCapsule));

                        _prevCapsule = capsule;
                    }

                    if (points.Count == 0)
                    {
                        if (_prevData.Count > 100)
                            _prevData.Clear();
                    }
                    else
                    {
                        _prevData.Clear();
                    }

                    return points;
                }

                switch (_settings.ScanMode)
                {
                    case ScanMode.HqScan:
                    {
                        var hqBytesToRead = (_serialPort.BytesToRead / HqNodeSize) * HqNodeSize;
                        if (hqBytesToRead == 0)
                            return new List<LidarPoint>(0);
                        data = new byte[hqBytesToRead];
                        _serialPort.Read(data, 0, data.Length);
                        var hqNodes = data.ParseHqNodes();
                        return HqScanToNormal(hqNodes);
                    }

                    case ScanMode.Standard:
                    {
                        var bytesToRead = _serialPort.BytesToRead;
                        data = new byte[bytesToRead];
                        _serialPort.Read(data, 0, data.Length);
                        return ParseStandardScanPackets(data);
                    }

                    case ScanMode.Boost:
                    case ScanMode.Sensitivity:
                    {
                        var bytesToRead = _serialPort.BytesToRead;

                        data = new byte[bytesToRead];
                        _serialPort.Read(data, 0, data.Length);
                        var points = new List<LidarPoint>();
                        _prevData.Add(data);
                        var aggregates = _prevData.SelectMany(x => x).ToArray();
                        var result = aggregates.WaitUltraCapsuledNode();
                        var resultCount = result.Count;
                        var reminder = new List<byte[]>();
                        while (result.Count > 0)
                        {
                            var item = result.Dequeue();
                            if (!item.IsRpLidarRespMeasurementSyncBitExp)
                            {
                                _isPreviousCapsuleDataRdy = false;
                                prev = null;
                                if (item.RemainderData != null)
                                    reminder.Add(item.RemainderData);
                            }

                            if (item.IsStartAngleSyncQ6)
                            {
                                var collection = UltraCapsuleToNormal(item.Value);
                                var exit = false;
                                for (var index = 0; index < collection.Count; index++)
                                {
                                    var lidarPoint = collection[index];
                                    if ((lidarPoint.Flag & DataResponseHelper.RpLidarRespMeasurementSyncBit) > 0)
                                    {
                                        _isPreviousCapsuleDataRdy = false;
                                        if (_render)
                                        {
                                            _render = false;
                                            exit = true;
                                        }
                                        else
                                        {
                                            _render = true;
                                        }
                                    }
                                }
                                if (exit)
                                    continue;
                                points.AddRange(collection);
                            }
                        }

                        if (resultCount == 0 || points.Count == 0)
                        {
                            if (_prevData.Count > 100)
                            {
                                _prevData.Clear();
                            }
                        }
                        else
                        {
                            _prevData.Clear();
                        }

                        if (reminder.Count > 0)
                        {
                            _prevData.Clear();
                            _prevData.AddRange(reminder);
                        }

                        return points;
                    }

                    case ScanMode.Dense:
                    {
                        var bytesToReadDense = _serialPort.BytesToRead;
                        data = new byte[bytesToReadDense];
                        _serialPort.Read(data, 0, data.Length);
                        var densePackets = data.WaitDenseCapsuledNode();
                        var densePoints = new List<LidarPoint>();
                        while (densePackets.Count > 0)
                        {
                            var packet = densePackets.Dequeue();
                            if (_prevDense != null)
                                densePoints.AddRange(DenseCapsuleToNormal(packet, _prevDense));
                            _prevDense = packet;
                        }
                        return densePoints;
                    }

                    case ScanMode.UltraDense:
                    {
                        var bytesToReadUd = _serialPort.BytesToRead;
                        data = new byte[bytesToReadUd];
                        _serialPort.Read(data, 0, data.Length);
                        var udPackets = data.WaitUltraDenseCapsuledNode();
                        var udPoints = new List<LidarPoint>();
                        while (udPackets.Count > 0)
                        {
                            var packet = udPackets.Dequeue();
                            if (_prevUltraDense != null)
                                udPoints.AddRange(UltraDenseCapsuleToNormal(packet, _prevUltraDense));
                            _prevUltraDense = packet;
                        }
                        return udPoints;
                    }
                }
            }
            return new List<LidarPoint>(0);
        }

        public void Start()
        {
            _isStop = false;
            while (!_isScanning)
            {
                try
                {
                    Connect();
                    StopScan();
                    switch (_settings.ScanMode)
                    {
                        case ScanMode.HqScan:
                            StartHqScan();
                            break;

                        case ScanMode.Boost:
                        case ScanMode.Sensitivity:
                        case ScanMode.Dense:
                        case ScanMode.UltraDense:
                            ForceScanExpress();
                            break;

                        default:
                            StartScan();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                    Thread.Sleep(2000);
                }
            }
        }

        public void Stop()
        {
            _isStop = true;
            StopScan();
        }

        /// <summary>
        /// Queries the device's standard and express scan sample rates.
        /// Returns null if the command is not supported by this device firmware.
        /// </summary>
        public SampleRateResponse GetSampleRate()
        {
            var response = SendRequest(Command.GetSampleRate);
            return response as SampleRateResponse;
        }

        /// <summary>
        /// Queries a device configuration value. The interpretation of the response depends on the sub-type.
        /// </summary>
        /// <param name="subType">The configuration property to query.</param>
        /// <param name="argument">Optional argument (e.g. scan mode ID for per-mode queries).</param>
        public LidarConfResponse GetLidarConf(LidarConfSubType subType, uint argument = 0)
        {
            if (!_isConnected)
                return null;

            _serialPort.DiscardInBuffer();

            bool hasArgument = argument != 0 || RequiresArgument(subType);
            byte[] payload = hasArgument ? new byte[8] : new byte[4];

            payload[0] = (byte)((uint)subType & 0xFF);
            payload[1] = (byte)(((uint)subType >> 8) & 0xFF);
            payload[2] = (byte)(((uint)subType >> 16) & 0xFF);
            payload[3] = (byte)(((uint)subType >> 24) & 0xFF);

            if (hasArgument)
            {
                payload[4] = (byte)(argument & 0xFF);
                payload[5] = (byte)((argument >> 8) & 0xFF);
                payload[6] = (byte)((argument >> 16) & 0xFF);
                payload[7] = (byte)((argument >> 24) & 0xFF);
            }

            _serialPort.SendCommand((byte)Command.GetLidarConf, payload);

            try
            {
                var descriptor = ReadResponseDescriptor();

                // Response payload: 4-byte sub-type echo followed by the actual data
                int totalLen = descriptor.ResponseLength;
                byte[] raw = Read(totalLen, 1000);

                byte[] data = totalLen > 4 ? new byte[totalLen - 4] : Array.Empty<byte>();
                if (data.Length > 0)
                    Array.Copy(raw, 4, data, 0, data.Length);

                return new LidarConfResponse { SubType = subType, RawPayload = data };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Sends a device reset command (0x40). The device reboots and enters idle state.
        /// Call Connect() again after ~2 seconds to re-establish communication.
        /// </summary>
        public void Reset()
        {
            _serialPort.SendRequest(Command.Reset);
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Queries the device for all supported scan modes via GetLidarConf.
        /// Returns an empty list if the device firmware does not support GetLidarConf (pre-1.17 firmware).
        /// </summary>
        public List<LidarScanMode> GetAllSupportedScanModes()
        {
            var modes = new List<LidarScanMode>();
            try
            {
                var countResp = GetLidarConf(LidarConfSubType.ScanModeCount);
                if (countResp == null) return modes;
                ushort count = countResp.ToUInt16();

                for (ushort modeId = 0; modeId < count; modeId++)
                {
                    var nameResp    = GetLidarConf(LidarConfSubType.ScanModeName,        modeId);
                    var usResp      = GetLidarConf(LidarConfSubType.ScanModeUsPerSample, modeId);
                    var maxDistResp = GetLidarConf(LidarConfSubType.ScanModeMaxDistance, modeId);
                    var ansResp     = GetLidarConf(LidarConfSubType.ScanModeAnsType,     modeId);

                    modes.Add(new LidarScanMode
                    {
                        Id          = modeId,
                        Name        = nameResp?.ToString() ?? $"Mode{modeId}",
                        UsPerSample = usResp?.ToFloat() ?? 0f,
                        MaxDistance = maxDistResp?.ToFloat() ?? 0f,
                        AnsType     = (byte)(ansResp?.ToUInt16() ?? 0)
                    });
                }
            }
            catch (Exception)
            {
                // Device doesn't support GetLidarConf — return empty list
            }
            return modes;
        }

        /// <summary>
        /// Queries the device's recommended (typical) scan mode.
        /// Returns null if the device does not support GetLidarConf.
        /// </summary>
        public LidarScanMode? GetTypicalScanMode()
        {
            try
            {
                var typicalResp = GetLidarConf(LidarConfSubType.ScanModeTypical);
                if (typicalResp == null) return null;
                ushort typicalId = typicalResp.ToUInt16();

                var allModes = GetAllSupportedScanModes();
                return allModes.Find(m => m.Id == typicalId) is LidarScanMode found ? found : (LidarScanMode?)null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool RequiresArgument(LidarConfSubType subType)
        {
            return subType == LidarConfSubType.ScanModeUsPerSample
                || subType == LidarConfSubType.ScanModeMaxDistance
                || subType == LidarConfSubType.ScanModeAnsType
                || subType == LidarConfSubType.ScanModeName;
        }
    }
}