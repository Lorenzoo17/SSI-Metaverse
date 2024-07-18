using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace JsonClasses {

    public enum VerifiableCredentialType {
        Identity,
        DriverLicense,
        HealthCertificate
    }

    #region SSI Classes
    // --------------------------------- MAIN STRUCTURES
    [Serializable]
    public class VerifiablePresentation {
        public List<VerifiableCredentialContainer> verifiableCredentials;
        public List<StandardVerifiableCredential> verifiableCredential_empty;
        public string holder;
        public string[] type;
        public string[] context;
        public string issuanceDate;
        public Proof proof;
    }
    [Serializable]
    public class VerifiableCredentialContainer {
        public string hash;
        public StandardVerifiableCredential verifiableCredential;
    }

    [Serializable]
    public class StandardVerifiableCredential {
        public CredentialSubject credentialSubject;
        public Issuer issuer;
        public string[] type;
        // public string[] context; // ??
        public string issuanceDate;
        public Proof proof;

        public StandardVerifiableCredential(CredentialSubject credentialSubject, Issuer issuer, string[] type, string issuanceDate, Proof proof) {
            this.credentialSubject = credentialSubject;
            this.issuer = issuer;
            this.type = type;
            this.issuanceDate = issuanceDate;
            this.proof = proof;
        }

        //create with string -> string parameters are passed and json is created in this way (make a new method in ClientLite to transmiss pure json string)
        public string GetSimpleString_FillSubject(string credentialSubjectContent) {
            return $"Issuer : {issuer.id}\nType : [{type[0]},{type[1]}]\nIssuanceDate : {issuanceDate}\nCredentialSubject : [{credentialSubjectContent}]"; // Proof must be inserted too
        }

        public string GetJsonString(string credentialSubjectContent) {
            string jsonString = string.Empty;
            /*
            jsonString = "{\n" +
                "credentialSubject: {\n" +
                credentialSubjectContent +
                "},\n" +
                "issuer: {\n" +
                "id: " + issuer.id + "\n" +
                "},\n" +
                "type: [" + type[0] + ", " + type[1] + "],\n" +
                "issuanceDate: " + "" + issuanceDate + ",\n" +
                "proof: {\n" +
                "type: " + proof.type + ",\n" +
                "jwt: " + proof.jwt + "\n" +
                "}\n" +
                "}";
            */
            jsonString = "{\n" +
                "\"credentialSubject\": {\n" +
                credentialSubjectContent +
                "},\n" +
                "\"issuer\": {\n" +
                "\"id\": \"" + issuer.id + "\"\n" +
                "},\n" +
                "\"type\": [\"" + type[0] + "\", \"" + type[1] + "\"],\n" +
                "\"issuanceDate\": " + "\"" + issuanceDate + "\",\n" +
                "\"proof\": {\n" +
                "\"type\": \"" + proof.type + "\",\n" +
                "\"jwt\": \"" + proof.jwt + "\"\n" +
                "}\n" +
                "}";
            

            return jsonString;
        }
    }

    // ---------------------------------- COMPONENTS OF MAIN STRUCTURE

    [Serializable]
    public class CredentialSubject {
        public string id;
        public string you;
        public int age;
        public string license;
        public int heartbeat;
        public int systolicPressure;
        public int diastolicPressure;

        public CredentialSubject(string id, string you, int age, string license, int heartbeat, int systolicPressure, int diastolicPressure) {
            this.id = id;
            this.you = you;
            this.age = age;
            this.license = license;
            this.heartbeat = heartbeat;
            this.systolicPressure = systolicPressure;
            this.diastolicPressure = diastolicPressure;
        }

        // Method used for the LitenetLib writer object when a credential must be transmitted from a user to another (used in NetExtension.cs)
        public static LiteNetLib.Utils.NetDataWriter LiteNetLib_WriterGenerator(LiteNetLib.Utils.NetDataWriter writer, StandardVerifiableCredential verifiableCredential) {
            writer.Put(verifiableCredential.credentialSubject.id);
            writer.Put(verifiableCredential.credentialSubject.you);
            writer.Put(verifiableCredential.credentialSubject.age);
            writer.Put(verifiableCredential.credentialSubject.license);
            writer.Put(verifiableCredential.credentialSubject.heartbeat);
            writer.Put(verifiableCredential.credentialSubject.systolicPressure);
            writer.Put(verifiableCredential.credentialSubject.diastolicPressure);

            return writer;
        }

        // Method used for the LitenetLib reader object when a credential must be transmitted from a user to another (used in NetExtension.cs)
        public static CredentialSubject LiteNetLib_WriteReaderResolver(ref LiteNetLib.Utils.NetDataReader reader) {
            string cs_id = reader.GetString();
            string cs_name = reader.GetString();
            int cs_age = reader.GetInt();
            string cs_license = reader.GetString();
            int cs_heartbeat = reader.GetInt();
            int cs_sys_pressure = reader.GetInt();
            int cs_dia_pressure = reader.GetInt();

            CredentialSubject cs = new(cs_id, cs_name, cs_age, cs_license, cs_heartbeat, cs_sys_pressure, cs_dia_pressure);
            return cs;
        }

        public override string ToString() {
            var result = "credentialSubject:\n";

            if (id != default(string))
                result += $"\"id\": \"{id}\"\n";

            if (you != default(string))
                result += $"\"you\": \"{you}\"\n";

            if (age != default(int))
                result += $"\"age\": {age}\n";

            if (license != default(string))
                result += $"\"license\": \"{license}\"\n";

            if (heartbeat != default(int))
                result += $"\"heartbeat\": {heartbeat}\n";

            if (systolicPressure != default(int))
                result += $"\"systolicPressure\": {systolicPressure}\n";

            if (diastolicPressure != default(int))
                result += $"\"diastolicPressure\": {diastolicPressure}\n";

            return result.TrimEnd();
        }

        public string GetSpecificContent() { // Everything available except for DID and name
            var result = "CredentialSubject:\n";

            if (age != default(int))
                result += $"Age: {age}\n";

            if (license != default(string))
                result += $"License: {license}\n";

            if (heartbeat != default(int))
                result += $"Heartbeat: {heartbeat}\n";

            if (systolicPressure != default(int))
                result += $"Systolic Pressure: {systolicPressure}\n";

            if (diastolicPressure != default(int))
                result += $"Diastolic Pressure: {diastolicPressure}\n";

            return result.TrimEnd();
        }

        public string[] GetFields() {
            string[] lines = ToString().Split(new[] { '\n' }, StringSplitOptions.None);

            string[] fields = new string[lines.Length - 1];
            for(int i = 1; i < lines.Length; i++) {
                fields[i - 1] = lines[i];
            }
            return fields;
        }
    
    }

    // ???????????????? ?????????????????? ???????????????? ?????????????
/*
    public class CredentialSubject {
        public Dictionary<string, object> Attributes { get; set; }

        // Constructor to initialize the dictionary
        public CredentialSubject() {
            Attributes = new Dictionary<string, object>();
        }

        // Indexer to access dictionary values easily
        public object this[string key] {
            get => Attributes.ContainsKey(key) ? Attributes[key] : null;
            set => Attributes[key] = value;
        }
    }
*/
    [Serializable]
    public class Issuer {
        public string id;

        public Issuer(string id) {
            this.id = id;
        }

        public override string ToString() {
            return id.ToString();
        }
    }

    [Serializable]
    public class Proof {
        public string type;
        public string jwt;

        public Proof(string type, string jwt) {
            this.type = type;
            this.jwt = jwt;
        }
    }
    #endregion

    #region Cloud classes

    [Serializable]
    public class GraphData {
        [Serializable]
        public struct TimeStampData {
            public string timestamp;
            public float value;

            public int GetDay() {
                if (DateTime.TryParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)) {
                    return dateTime.Day;
                }
                throw new FormatException("Invalid timestamp format");

            }

            public int GetMonth() {
                if (DateTime.TryParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)) {
                    return dateTime.Month;
                }
                throw new FormatException("Invalid timestamp format");

            }

            public int GetHours() {
                if (DateTime.TryParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)) {
                    return dateTime.Hour;
                }
                throw new FormatException("Invalid timestamp format");

            }

            public int GetMinutes() {
                if (DateTime.TryParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)) {
                    return dateTime.Minute;
                }
                throw new FormatException("Invalid timestamp format");
            }

            public int GetSeconds() {
                if (DateTime.TryParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)) {
                    return dateTime.Second;
                }
                throw new FormatException("Invalid timestamp format");
            }
        }

        public TimeStampData[] timestampData;
    }

    #endregion
}
