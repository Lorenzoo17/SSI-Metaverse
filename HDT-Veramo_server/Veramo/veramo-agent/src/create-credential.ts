import { agent } from './veramo/setup.js'

async function main() {
  const identifier = await agent.didManagerGetByAlias({ alias: 'Gianni' }) // retrieve DID associated with the specified alias

  const verifiableCredential = await agent.createVerifiableCredential({
    credential: {
      issuer: { id: identifier.did },
      type:['VerifiableCredential', 'HealthCertificate'],
      credentialSubject: {
        id: identifier.did,
        you: 'Gianni',
        heartbeat: '80',
        systolicPressure: '120',
        diastolicPressure : '65'
      },
    },
    proofFormat: 'jwt',
  })
  console.log(`New credential created`)
  console.log(JSON.stringify(verifiableCredential, null, 2))

  await agent.dataStoreSaveVerifiableCredential({ verifiableCredential: verifiableCredential}) // Save the credential in database.sqlite after creating it
  console.log("Credential saved")
}

main().catch(console.log)