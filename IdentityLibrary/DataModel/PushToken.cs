using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityLibrary.DataModel
{
    public class PushToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public OS OS { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual AspNetUsers User { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateIn { get; set; }
    }
    public enum OS
    {
        Android,
        Apple
    }
}
