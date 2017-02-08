using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace GtaLauncher.confuguration
{
    class MagicAttribute : Attribute { }

    public class Configuration : INotifyPropertyChanged
    {
        [Magic]
        public string GamePath { get; set; }

        [Magic]
        public string ServerAdress { get; set; }

        [Magic]
        public string ServerPort { get; set; }

        private static Configuration _instanse;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); // некоторые из нас здесь используют Dispatcher, для безопасного взаимодействия с UI thread
        }

        private Configuration()
        {
            bind();
        }

        private void bind()
        {
            PropertyChanged += SaveCofiguration;
        }

        private void SaveCofiguration(object sender, PropertyChangedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("Configuration.json"))
            {
                writer.Write(JsonConvert.SerializeObject(this));
            }
        }

        public static Configuration Instanse
        {
            get
            {
                if(_instanse == null)
                {
                    if(File.Exists("Configuration.json"))
                    {
                        using (StreamReader reader = new StreamReader("Configuration.json"))
                        {
                            string json = reader.ReadLine();
                            try
                            {
                                _instanse = JsonConvert.DeserializeObject<Configuration>(json);
                                _instanse.bind();
                            }
                            catch (Exception ex)
                            {
                                _instanse = new Configuration();
                            }
                        }
                    }
                    else
                    {
                        _instanse = new Configuration();
                    }
                }
                return _instanse;
            }
        }
    }
}
