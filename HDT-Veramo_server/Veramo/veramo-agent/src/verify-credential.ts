import { agent } from './veramo/setup.js'

async function main() {
  const result = await agent.verifyCredential({
    credential: {
      credentialSubject: {
        you: 'Rock',
        id: 'did:web:example.com',
      },
      issuer: {
        id: 'did:ethr:sepolia:0x038a5b84dbbaa10f3d31c9c68ea174883c0df96e9248d4af012c085198f3fd751a',
      },
      type: ['VerifiableCredential'],
      '@context': ['https://www.w3.org/2018/credentials/v1'],
      issuanceDate: '2024-06-04T09:00:53.000Z',
      proof: {
        type: 'JwtProof2020',
        jwt: 'eyJhbGciOiJFUzI1NksiLCJ0eXAiOiJKV1QifQ.eyJ2YyI6eyJAY29udGV4dCI6WyJodHRwczovL3d3dy53My5vcmcvMjAxOC9jcmVkZW50aWFscy92MSJdLCJ0eXBlIjpbIlZlcmlmaWFibGVDcmVkZW50aWFsIl0sImNyZWRlbnRpYWxTdWJqZWN0Ijp7InlvdSI6IlJvY2sifX0sInN1YiI6ImRpZDp3ZWI6ZXhhbXBsZS5jb20iLCJuYmYiOjE3MTc0OTE2NTMsImlzcyI6ImRpZDpldGhyOnNlcG9saWE6MHgwMzhhNWI4NGRiYmFhMTBmM2QzMWM5YzY4ZWExNzQ4ODNjMGRmOTZlOTI0OGQ0YWYwMTJjMDg1MTk4ZjNmZDc1MWEifQ.g-_gjmWbofzZTWDANvlv32Glbg273r_tUIu3GF9gI1xAF8kyOSmGw1ypth_GH9yUamKbZ3Or-CB9ASAPmigBcg',
      },
    },
  })
  console.log(`Credential verified`, result.verified)
}

main().catch(console.log)