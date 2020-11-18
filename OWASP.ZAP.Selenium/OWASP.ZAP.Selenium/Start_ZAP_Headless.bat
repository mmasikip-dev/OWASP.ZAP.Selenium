cd C:\Program Files\OWASP\Zed Attack Proxy
zap.sh -daemon -host %1 -port %2 -config api.addrs.addr.regex=true -config api.disablekey=true