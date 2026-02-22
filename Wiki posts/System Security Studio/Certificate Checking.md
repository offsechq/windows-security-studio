# Certificate Checking | System Security Studio

<p align="center">
</p>

This page in the [System Security Studio App](https://github.com/OFFSECHQ/windows-security-studio/wiki/Harden-System-Security) is dedicated to enumerating every certificate across all stores in the **Local Machine** and **Current User** scopes. Certificates can be sorted, searched, and removed as needed.

There is a key capability identifying certificates that are **not rooted** to [the Microsoft's Trusted Roots list](https://learn.microsoft.com/security/trusted-root/participants-list) and presents options for remediation. When evaluating trust, the app does **not** rely on the system's certificate store; it uses an independent, built-in trust anchor to validate certificates, so its checks remain unaffected by a potentially compromised system store.
