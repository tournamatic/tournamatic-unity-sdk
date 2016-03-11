using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournamatic.Model;

namespace Tournamatic
{
    public class Browse
    {
        public IList<Tournament> GetAll()
        {
            return new List<Tournament> {
                new Tournament {
                    Title = "Test 1"
                }
            };
        }
    }
}
