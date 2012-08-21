using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NVelocityDemo
{
    class Program
    {
        public enum Gender
        {
            Unknown,
            Male,
            Female
        }

        /// <summary>
        /// Demo entity of user
        /// </summary>
        public class UserEntity
        {
            public Gender Gender { get; set; }
            public DateTime Birthdate { get; set; }
            public int Age { get { return (DateTime.Now.Year - this.Birthdate.Year); } }
            public string FullName { get; set; }

            public string[] Phones { get; set; }
        }

        static void Main(string[] args)
        {
            var user = new UserEntity { Birthdate = DateTime.Parse("1/1/1970"), Gender= Gender.Male, FullName = "First Last", Phones = new string[] {"123","456"} };

            Console.WriteLine("user data:");

            var propertyinfos = user.GetType().GetProperties();
            foreach (var propinfo in propertyinfos)
            {
                Console.WriteLine(propinfo.Name + ": " + propinfo.GetValue(user, null));

            }

            var fieldinfo = user.GetType().GetFields();
            foreach (var propinfo in fieldinfo)
            {
                Console.WriteLine(propinfo.Name + ": " + propinfo.GetValue(user));

            }

            Console.WriteLine("Template parsing and execution:");

            string template =
@"Template
Gender: $user.Gender
FullName: $user.FullName
Birthdate: $user.Birthdate
Age: $user.Age

Phones:
#if ($user.Phones.Length > 0)
#printphones($user.Phones)
#end

#macro (printphones $phones)
#set( $no=1)
#foreach ($ph in $phones)
Phone $no: $ph
#set( $no = $no + 1)
#end
#end


";

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["user"] = user;
            Console.WriteLine(TemplateEngine.Process(dict, template));


        }
    }
}
