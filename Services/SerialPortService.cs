using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace io_simulation_wpf.Services
{
    public class SerialPortService
    {
        private SerialPort? port;

        public string PortName { get; set; }
        public int BaudRate { get; set; }


        private StringBuilder _readBuffer = new StringBuilder();
        public event EventHandler<string>? LineReceived;

        public SerialPortService()
        {

        }

        public void Open()
        {
            if (port is not null) port.Close();
            port = new SerialPort(PortName, BaudRate);
            port.DataReceived += OnDataRecieved;
            port.Open();
        }

        private void OnDataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Solange noch Daten im Puffer sind
                while (port.BytesToRead > 0)
                {
                    string chunk = port.ReadExisting();
                    // Durchlaufen wir jedes Zeichen
                    foreach (char c in chunk)
                    {
                        if (c == '\n' || c == '\r')
                        {
                            // Wir haben ein Paket/Zeile beendet
                            if (_readBuffer.Length > 0)
                            {
                                string line = _readBuffer.ToString();
                                _readBuffer.Clear();
                                // Hier Event feuern (oder parseLine aufrufen)
                                LineReceived?.Invoke(this, line);
                            }
                            // Falls du CR+LF hast, ignorieren wir das zweite
                        }
                        else
                        {
                            // Normales Zeichen => an den Buffer anhängen
                            _readBuffer.Append(c);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung, z.B. Loggen
            }
        }

        public void Write(string data)
        {
            if (port is not null && port.IsOpen)
            {
                port.Write(data);
            }
        }

        public void WriteLine(string data)
        {
            if (port is not null && port.IsOpen)
            {
                port.WriteLine(data);
            }
        }

        public void Close()
        {
            if (port is not null && port.IsOpen)
            {
                port.Close();
            }
        }

        public IEnumerable<string> GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }


    }
}
