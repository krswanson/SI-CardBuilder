﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes
{
    public class SimpleEventArgs : EventArgs
    {
        public object data;

        public SimpleEventArgs(object data)
        {
            this.data = data;
        }
    }
}
