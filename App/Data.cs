using System;
using WebCommander.App.Controller;
namespace WebCommander.App;

    class Data
    {
        private string str;
        private SimplerController sc = new SimplerController("trie");
        public Data(string str)
        {
            this.str = str;
        }

        public string Str { get => str; set => str = value; }

        public override bool Equals(object? obj)
        {
            return obj is Data data &&
                   str == data.str;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(str);
        }
    }

