## How to Generate New Certificates for Acceptance Tests

### TL/DR
1. From a bash prompt: ``./certs-create.sh``

### What This Does
1. The above command will result in the following certificates to be generated:
   * snakeoil-ca-1 (the CA or "signing" certificate)
   * kafka1 (the cert used to identify the first broker)
   * client (the cert used to identify a client)
1. All certs except the "snakeoil" cert will be signed by the snakeoil cert
1. These certs will then be added to a Java keystore acting as the truststore (store of trusted certs) to be used by the brokers

### Changing What's Generated
* The list of certs created (other than the snakeoil CA cert) can be changed by changing the list of certificate names following the ``in`` keyword in the following line from **certs-create.sh**:
  * ``for i in kafka1 client``
