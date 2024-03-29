﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AMS_Base
{
    public class AboutMySoftware
    {
        private  static Dictionary<string, AboutMySoftware> AllDefinitionsDict = new Dictionary<string, AboutMySoftware>();
        public static DateTime[] Dates
        {
            get
            {
                return AllDefinitions.Select(it => it.Value.DateGenerated.Date).OrderByDescending(it=>it).ToArray();
            }
        }
        
        public static KeyValuePair<string,AboutMySoftware>[] AllDefinitions
        {
            get
            {
                return AllDefinitionsDict.OrderByDescending(it=>it.Value.DateGenerated).ToArray();
            }
        }
        public static void AddDefinition(string name , AboutMySoftware soft)
        {
            lock (AllDefinitionsDict)
            {
                AllDefinitionsDict[name] = soft;
            }
        }
        private List<VersionReleased> backup;
        public VersionReleased[] Versions
        {
            get
            {
                return backup.ToArray();
            }
            set
            {
                backup =  new List<VersionReleased>(value);
            }
        }
        public AboutMySoftware AddVersion(VersionReleased v)
        {
            this.backup.Add(v);
            return this;
        }
        public string EnvironmentVars { get; set; }
        public const string NotFoundLink = "https://ignatandrei.github.io/RSCG_AMS/runtimeMessages/NotFound.md";
        public AboutMySoftware()
        {
            backup = new List<VersionReleased>();
            CommitId = "not in a CI run";
            CISourceControl = "not in a CI run";
            var strEnv = "";
            foreach (var item in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User).Keys)
            {
                strEnv += ";User_"+item?.ToString();
            }
            foreach (var item in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process).Keys)
            {
                strEnv += ";Process_" + item?.ToString();
            }
            foreach (var item in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine).Keys)
            {
                strEnv += ";Machine_" + item?.ToString();
            }
            EnvironmentVars = strEnv;
            RepoUrl = NotFoundLink;
            SourceCommit = NotFoundLink;
            User = Environment.UserName;
            DateGenerated = DateTime.UtcNow;
            IsInCI = false;
        }
        public string User { get; set; }
        public string Version { get; set; }
        public string Authors { get; set; }
        public bool IsInCI { get; set; }
        public string SourceCommit { get; set; }
        public string CISourceControl { get; set; }
        public string AssemblyName { get; set; }
        public DateTime DateGenerated { get; set; }
        public string CommitId { get; set; }
        public string RepoUrl { get; set; }
    }

}
