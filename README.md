# OWASP.ZAP.Selenium

## How to setup OWASP ZAP Proxy

1. Download and install OWASP ZAP
    - Requires JRE 1.8
2. Install OWASP ZAP certificate
    - Go to Tools > Options > Dynamic SSL Certificates
    - Click Save
    - Install saved certificate
3. Note the API Key
    - Go to Tools > Options > API
    - Note API Key
4. Setup Proxy
    - Go to Tools > Options > Local Proxies
    - Enter '127.0.0.1' on Address
