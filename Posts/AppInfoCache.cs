using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrugalCafe
{
    public class AppInfo
    {
        public AppInfo()
        {
            Executables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValid(AppInfo app)
        {
            return true;
        }

        public HashSet<string> Executables { get; }
    }

    internal class AppInfoCache
    {
        private static List<AppInfo> CollectNonUwpAppInfos(List<AppInfo> cache)
        {
            List<AppInfo> list = new List<AppInfo>();
            
            if (cache != null)
            {
                list.AddRange(cache.Where(AppInfo.IsValid));
            }
            
            List<AppInfo> list2 = RpcMethods.UpdateAppCache();

            if (list2.Count > 0)
            {
                List<AppInfo> results = new List<AppInfo>();

                foreach (var  a in list)
                {
                    bool add = true;

                    foreach (var f in list2)
                    {
                        if (a.Executables.Overlaps(f.Executables))
                        {
                            add = false;
                            break;
                        }
                    }

                    if (add)
                    {
                        results.Add(a);
                    }
                }

                results.AddRange(list2);

                return results;
            }

            return list;
        }
    }

    public static class RpcMethods
    {
        public static bool EqualIgnoreCase(this string source, string target)
        {
            return source.Equals(target, StringComparison.InvariantCultureIgnoreCase);
        }

        public static List<AppInfo> UpdateAppCache()
        {
            return new List<AppInfo>();
        }
    }
}
