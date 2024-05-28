using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.Model
{
    public class NotesModel
    {
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        public string Colour { get; set; } = "";
       
    }
}
