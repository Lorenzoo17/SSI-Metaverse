using UnityEngine;
using LiteNetLib.Utils;
namespace Extension { //Per utilizzare queste estensioni devo importare il namespace Extension
    public static class NetExtension {

        public static void Put(this NetDataWriter writer, Vector3 vector) { //Definizione di metodo extension, con this vado a specificare il tipo di dato a cui il metodo fa riferimento, in questo modo facendo writer.Put(vector) --> posso direttamente utilizzare questo metodo, come se facesse parte della classe NetDataWriter 
            writer.Put(vector.x);
            writer.Put(vector.y);
            writer.Put(vector.z);
        }

        public static void Put(this NetDataWriter writer, Vector2 vector) { //Definizione di metodo extension, con this vado a specificare il tipo di dato a cui il metodo fa riferimento, in questo modo facendo writer.Put(vector) --> posso direttamente utilizzare questo metodo, come se facesse parte della classe NetDataWriter 
            writer.Put(vector.x);
            writer.Put(vector.y);
        }

        public static void Put(this NetDataWriter writer, Quaternion rotation) { //Per trasmettere quaternioni per rotazioni
            writer.Put(rotation.x);
            writer.Put(rotation.y);
            writer.Put(rotation.z);
            writer.Put(rotation.w);
        }

        public static void Put(this NetDataWriter writer, Vector3 headPosition, Quaternion headRotation, Vector3 rightArmPosition, Quaternion rightArmRotation, Vector3 leftArmPosition, Quaternion leftArmRotation) {
            writer.Put(headPosition);
            writer.Put(headRotation);
            writer.Put(rightArmPosition);
            writer.Put(rightArmRotation);
            writer.Put(leftArmPosition);
            writer.Put(leftArmRotation);
        }

        public static Vector3 GetVector3(this NetDataReader reader) {
            Vector3 vector;
            vector.x = reader.GetFloat();
            vector.y = reader.GetFloat();
            vector.z = reader.GetFloat();

            return vector;
        }

        public static Vector3 GetVector2(this NetDataReader reader) {
            Vector2 vector;
            vector.x = reader.GetFloat();
            vector.y = reader.GetFloat();

            return vector;
        }

        public static Quaternion GetQuaternion(this NetDataReader reader) {
            Quaternion rotation;
            rotation.x = reader.GetFloat();
            rotation.y = reader.GetFloat();
            rotation.z = reader.GetFloat();
            rotation.w = reader.GetFloat();

            return rotation;
        }

        public static void GetRemoteVisualSync(this NetDataReader reader, out Vector3 headPosition, out Quaternion headRotation, out Vector3 rightArmPosition, out Quaternion rightArmRotation, out Vector3 leftArmPosition, out Quaternion leftArmRotation) {
            headPosition = reader.GetVector3();
            headRotation = reader.GetQuaternion();
            rightArmPosition = reader.GetVector3();
            rightArmRotation = reader.GetQuaternion();
            leftArmPosition = reader.GetVector3();
            leftArmRotation = reader.GetQuaternion();
        }


        // SSI METHODS (remember to update after changing JsonClasses.cs!!!)

        public static void Put(this NetDataWriter writer, JsonClasses.StandardVerifiableCredential verifiableCredential) {
            // credential subject writer
            // Writing is managed in CredentialSubject in order to avoid modyfing this code when a change in CredentialSubject is made
            writer = JsonClasses.CredentialSubject.LiteNetLib_WriterGenerator(writer, verifiableCredential);

            // Issuer
            writer.Put(verifiableCredential.issuer.id);

            // Type (2 types, the first which identifies the verifiable type, the second identifies the content of credential)
            writer.Put(verifiableCredential.type[0]);
            writer.Put(verifiableCredential.type[1]);

            // Issuance date
            writer.Put(verifiableCredential.issuanceDate);

            // Proof
            writer.Put(verifiableCredential.proof.type);
            writer.Put(verifiableCredential.proof.jwt);
        }

        public static JsonClasses.StandardVerifiableCredential GetVerifiableCredential(this NetDataReader reader) {
            // read credential subject
            // Reading is managed in CredentialSubject in order to avoid modyfing this code when a change in CredentialSubject is made
            JsonClasses.CredentialSubject cs = JsonClasses.CredentialSubject.LiteNetLib_WriteReaderResolver(ref reader);

            // read issuer
            string issuer_id = reader.GetString();

            // read type
            string type_0 = reader.GetString();
            string type_1 = reader.GetString();

            string[] type_array = { type_0, type_1 }; 

            // Issuance date
            string date = reader.GetString();

            // Proof
            string proof_type = reader.GetString();
            string proof = reader.GetString();

            return new JsonClasses.StandardVerifiableCredential(cs, new JsonClasses.Issuer(issuer_id), type_array, date, new JsonClasses.Proof(proof_type, proof));
        }
    }
}