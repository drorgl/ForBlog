using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace FormatBenchmarks
{
    /// <summary>
    /// Compiled String Format
    /// <para>Compiles a string format into a standard structure which helps to build the output string up to 12% faster than string.format</para>
    /// </summary>
    class CompiledString
    {
        /// <summary>
        /// Compiled format
        /// </summary>
        private struct CompiledFormat
        {
            /// <summary>
            /// Contains the Parts which do not change
            /// </summary>
            public string[] Parts;
            /// <summary>
            /// Contains the Parameter Id (location) for each variable
            /// </summary>
            public int[] ParamId;
            /// <summary>
            /// Contains the maximum parts
            /// </summary>
            public int MaxPart;
        }

        /// <summary>
        /// a cache dictionary of all compiled strings
        /// </summary>
        private static ConcurrentDictionary<string, CompiledFormat> m_cache = new ConcurrentDictionary<string, CompiledFormat>();

        /// <summary>
        /// Compiles a format to CompiledFormat, uses the m_cache dictionary to store/load CompiledFormats
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private static CompiledFormat Compile(string template)
        {
            CompiledFormat cf;
            if (m_cache.TryGetValue(template, out cf))
                return cf;

            cf = new CompiledFormat();

            string[] parts = new string[10];
            int[] paramIds = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };


            int currentPart = 0;
            bool insideBrackets = false;

            string tempstr = string.Empty;
            string tempParamId = string.Empty;

            int templatelength = template.Length;

            for (int i = 0; i < templatelength; i++)
            {
                char currentC = template[i];
                if (currentC == '{')
                {
                    insideBrackets = true;

                    if (parts.Length -1 == currentPart)
                    {
                        Array.Resize(ref parts, parts.Length * 2);
                        Array.Resize(ref paramIds, paramIds.Length * 2);
                        //nullify the array
                        for (int z = currentPart; z < paramIds.Length; z++)
                            paramIds[z] = -1;
                    }

                    parts[currentPart] = tempstr;


                    tempstr = "";

                    tempParamId = "";


                    continue;
                }
                else if (currentC == '}')
                {
                    insideBrackets = false;

                    paramIds[currentPart] = int.Parse(tempParamId);

                    currentPart++;

                    continue;
                }
                else
                {
                    if (insideBrackets == false)
                        tempstr = tempstr + currentC;
                    else
                        tempParamId = tempParamId + currentC;
                }
            }

            parts[currentPart] = tempstr;

            cf.MaxPart = currentPart + 1;
            cf.ParamId = paramIds;
            cf.Parts = parts;

            m_cache[template] = cf;
            return cf;
        }

        /// <summary>
        /// Compiled String.Format equivalent
        /// <para>Fastest</para>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Format(string template, params object[] values)
        {
            CompiledFormat cf = Compile(template);

            StringBuilder sb = new StringBuilder(template.Length + (values.Length * 5));
            var maxpart = cf.MaxPart;
            for (int i = 0; i < maxpart; i++)
            {
                //if (cf.Parts.Length > i)
                sb.Append(cf.Parts[i]);

                if ((cf.ParamId[i]) != -1)
                    sb.Append(values[cf.ParamId[i]]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Compiled String.Format equivalent
        /// <para>Uses a fixed array and string.Concat almost equivalent in speed to Format</para>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string FormatStringConcat(string template, params object[] values)
        {
            CompiledFormat cf = Compile(template);

            string[] compiled = new string[cf.MaxPart * 2];
            int counter = 0;
            object val;
            for (int i = 0; i < cf.MaxPart; i++)
            {
                compiled[counter] = cf.Parts[i];
                counter++;
                if ((cf.ParamId[i]) != -1)
                {
                    val = values[cf.ParamId[i]];
                    if (val is string)
                        compiled[counter] = val as string;
                    else
                        compiled[counter] = val.ToString();
                    counter++;
                }
            }

            return string.Concat(compiled);

        }


        public static string FormatList(string template, params object[] values)
        {
            CompiledFormat cf = Compile(template);

            //string[] compiled = new string[cf.MaxPart * 2];
            List<string> compiledList = new List<string>();

            //int counter = 0;
            object val;
            for (int i = 0; i < cf.MaxPart; i++)
            {
                compiledList.Add(cf.Parts[i]);
                if ((cf.ParamId[i]) != -1)
                {
                    val = values[cf.ParamId[i]];
                    if (val is string)
                        compiledList.Add(val as string);
                    else
                        compiledList.Add(val.ToString());
                }
            }

            return string.Concat(compiledList.ToArray());

        }

        public static string FormatConcat(string template, params object[] values)
        {
            CompiledFormat cf = Compile(template);

            //string[] compiled = new string[cf.MaxPart * 2];
            string retval = string.Empty;
            int counter = 0;
            object val;
            for (int i = 0; i < cf.MaxPart; i++)
            {
                retval += cf.Parts[i];
                counter++;
                if ((cf.ParamId[i]) != -1)
                {
                    val = values[cf.ParamId[i]];
                    if (val is string)
                        retval += val as string;
                    else
                        retval += val.ToString();
                    counter++;
                }
            }

            return retval;

        }

        private struct Comparts
        {
            public int[] StartP;
            public int[] LenP;
            public int[] ParamId;
            public int Len;
        }

        private static Comparts CompileParts(string template)
        {
            Comparts cf;
            if (m_cachecomp.TryGetValue(template, out cf))
                return cf;

            cf = new Comparts();

            int[] StartP = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            int[] LenP = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            int[] ParamId = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

            int currentPart = 0;
            bool insideBrackets = false;

            string tempParamId = string.Empty;

            int templatelength = template.Length;

            for (int i = 0; i < templatelength; i++)
            {
                char currentC = template[i];
                if (currentC == '{')
                {
                    insideBrackets = true;

                    if (StartP.Length == currentPart)
                    {
                        Array.Resize(ref StartP, StartP.Length * 2);
                        Array.Resize(ref LenP, LenP.Length * 2);
                        Array.Resize(ref ParamId, ParamId.Length * 2);
                        //nullify the array
                        for (int z = currentPart; z < StartP.Length; z++)
                        {
                            StartP[z] = -1;
                            LenP[z] = -1;
                            ParamId[z] = -1;
                        }
                    }

                    //parts[currentPart] = tempstr;


                    //tempstr = "";

                    tempParamId = "";
                    StartP[currentPart] = i;

                    continue;
                }
                else if (currentC == '}')
                {
                    insideBrackets = false;

                    LenP[currentPart] = i - StartP[currentPart];

                    ParamId[currentPart] = int.Parse(tempParamId);

                    currentPart++;

                    continue;
                }
                else
                {
                    if (insideBrackets == true)
                        tempParamId = tempParamId + currentC;
                }
            }

            cf.StartP = StartP;
            cf.LenP = LenP;
            cf.ParamId = ParamId;
            cf.Len = currentPart;

            m_cachecomp[template] = cf;
            return cf;
        }

        private static ConcurrentDictionary<string, Comparts> m_cachecomp = new ConcurrentDictionary<string, Comparts>();

        public static string FormatInsertRemove(string template, params object[] values)
        {
            Comparts cp = CompileParts(template);

            //string tmp = string.Empty;// template;
            StringBuilder sb = new StringBuilder(template.Length * 2);
            var curpos = 0;
            for (int i = 0; i < cp.Len; i++)
            {
                sb.Append(template.Substring(curpos, cp.StartP[i] - curpos));
                //tmp += template.Substring(curpos, cp.StartP[i] - curpos);
                sb.Append(values[cp.ParamId[i]]);
                //tmp += values[cp.ParamId[i]].ToString();
                curpos = cp.LenP[i] + cp.StartP[i];
                //tmp = tmp.Remove(cp.StartP[i], cp.LenP[i]).Insert(cp.StartP[i], values[cp.ParamId[i]].ToString());
                //sb.Remove(cp.StartP[i], cp.LenP[i]);
                //sb.Insert(cp.StartP[i],values[cp.ParamId[i]]);
            }
            return sb.ToString();

        }


        public static string FormatReplace(string template, params object[] values)
        {
            //Comparts cp = CompileParts(template);

            StringBuilder sb = new StringBuilder(template, 1024);
            for (int i = 0; i < values.Length; i++)
                sb.Replace("{" + i + "}", values[i].ToString());
            return sb.ToString();

        }


        public static string FormatFly(string template, params object[] values)
        {
            bool insideBrackets = false;

            StringBuilder sb = new StringBuilder();

            string tempParamId = string.Empty;

            int templatelength = template.Length;

            int frompos = 0;

            for (int i = 0; i < templatelength; i++)
            {
                char currentC = template[i];
                if (currentC == '{')
                {
                    sb.Append(template.Substring(frompos, i - frompos));
                    insideBrackets = true;
                    tempParamId = "";
                    continue;
                }
                else if (currentC == '}')
                {
                    frompos = i + 1;
                    insideBrackets = false;
                    sb.Append(values[int.Parse(tempParamId)]);
                    continue;
                }
                else
                {
                    if (insideBrackets == true)
                        tempParamId += currentC;

                    //if (insideBrackets == false)
                    //    sb.Append(currentC);
                    //else
                    //    tempParamId = tempParamId + currentC;
                }
            }
            if (frompos < template.Length)
                sb.Append(template.Substring(frompos, template.Length - frompos));

            return sb.ToString();
        }

        //todo, add 1 more, copy byte by byte, if inside a {}, copy value.
        public static string FormatByteCopy(string template, params object[] values)
        {
            bool insideBrackets = false;

            //int templatelength = template.Length;
            char[] templatechars = template.ToCharArray();

            char[] retval = new char[templatechars.Length * 2];
            int retvalpos = 0;

            string tempParamId = string.Empty;

            

            for (int i = 0; i < templatechars.Length; i++)
            {
                //char currentC = template[i];
                switch (templatechars[i])
                {
                    case '{':
                        {
                            insideBrackets = true;
                            tempParamId = "";
                            continue;
                        }
                        break;
                    case '}':
                        {
                            insideBrackets = false;

                            var valuecopy = values[int.Parse(tempParamId)];
                            if (valuecopy == null)
                                continue;

                            string valuestring = valuecopy.ToString();
                            for (int vc = 0; vc < valuestring.Length; vc++)
                            {
                                if (retval.Length == retvalpos)
                                {
                                    Array.Resize(ref retval, retvalpos * 2);
                                }

                                retval[retvalpos++] = valuestring[vc];
                            }

                            continue;
                        }
                        break;
                    default:
                        {
                            if (insideBrackets == false)
                            {
                                if (retval.Length == retvalpos)
                                {
                                    Array.Resize(ref retval, retvalpos * 2);
                                }
                                retval[retvalpos++] = templatechars[i];
                            }
                            else
                                tempParamId = tempParamId + templatechars[i];
                        }
                        break;

                }
            }

            return new string (retval);
        }

    }
}
