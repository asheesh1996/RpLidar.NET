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
    /// <summary>
    /// RPLidar A2, A3
    /// </summary>
    public class RpLidarSerialDevice : ILidarService
    {/// <summary>
     /// Ctor
     /// </summary>
     /// <param name="settings"></param>
     /// <param name="localLogger"></param>
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
        /// Connect to serial port
        /// </summary>
        public void Connect()
        {
            if (this._isConnected)
            {
                Console.WriteLine("already connected");
            }
            else
            {
                try
                {
                    Console.WriteLine($"Connected to RPLidar on {_serialPort.PortName}");
                    _serialPort.Open();
                    _isConnected = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error lidar", e);
                }
                // Connect Serial
                while (!_isConnected)
                {
                    //var ports = SerialPort.GetPortNames().ToList();
                    //foreach (var portName in ports)
                    {
                        try
                        {
                            //CreateSerial(portName);
                            _serialPort.Open();
                            _isConnected = true;
                            Console.WriteLine($"Connected to RPLidar on {_serialPort.PortName}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error lidar: {_serialPort.PortName}", e);
                        }
                    }
                    Thread.Sleep(1000);
                }
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
                    var scan = new RplidarPayloadExpressScan()
                    {
                        working_mode = _settings.Type,
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
            this.SendCommand((byte)0x25);
            Thread.Sleep(20);
            StopMotor();
        }

        /// <summary>
        /// Thread used for scanning
        /// Populates a list of Measurements, and adds that list to
        /// </summary>
        public void ScanThread()
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

        public byte[] Read(int len, int timeout)
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
                            var distance = dist_q2[cpos];
                            node.Angle = angle * 90f / 16384f;
                            node.Distance = distance > (ushort.MaxValue) ? 0 : (UInt16)(distance);
                            //if (node.Distance < 0)
                            //	node.Distance = -node.Distance;
                            if ((flag & 1) > 0)
                            {
                                node.StartFlag = true;
                            }

                            if (node.Distance > 0 && node.Quality > 0)
                            {
                                node.Distance = node.Distance / 4f;
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

        private bool _render;

        private readonly List<byte[]> _prevData = new List<byte[]>(100);

        private List<LidarPoint> WaitAndParseData()
        {
            var bufSize = 2002;
            if (_settings.Type != 0)
                bufSize = 132 * 3;
            byte[] data = null;
            var i = 0;
            while (_serialPort.BytesToRead < bufSize && _isScanning && i < 5)
            {
                Thread.Sleep(5);
                i++;
            }
            if (_serialPort.BytesToRead > 0)
            {
                switch (_settings.Type)
                {
                    case 3:
                    case 4:
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
                    switch (_settings.Type)
                    {
                        case 4:
                        case 3:
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
    }
}