using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Settings
{
    public class JsonConfigHelper
    {
        private static JsonConfigHelper _jsonConfigHelper;

        private JObject _jsonObj;

        private  JsonConfigHelper()
        {
            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}\Configs\Config.json";

            string jsonText = File.ReadAllText(path);

            _jsonObj = JsonConvert.DeserializeObject<JObject>(jsonText);
        }

        public static JsonConfigHelper Instance
        {
            get {
                if (_jsonConfigHelper == null)
                {
                    _jsonConfigHelper = new JsonConfigHelper();
                }

                return _jsonConfigHelper;
            }
        }

        public string GetValue(string key)
        {
           var val = _jsonObj[key];

            if (val == null)
                return "";
            else
                return val.ToString();
        }
    }
}
