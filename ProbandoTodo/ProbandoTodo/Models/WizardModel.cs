using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProbandoTodo.Models
{
    public class WizardModel
    {
        public ThemeWzModel theme { get; set; }
        public string avatarImg { get; set; }
        public PhraseWzModel personalMessage { get; set; }
    }

    public class ThemeWzModel
    {
        public string name { get; set; }
        public string image { get; set; }
    }

    public class PhraseWzModel
    {
        public string phrase { get; set; }
        public string color { get; set; }
    }
}