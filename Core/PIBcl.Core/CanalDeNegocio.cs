using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIBcl.Core
{
    public class CanalDeNegocio
    {
        private string _nome = "";

        [JsonProperty(PropertyName = "nome")]
        public string Nome { get { return _nome?.ToLower(); } set { _nome = value; } }
    }
}
