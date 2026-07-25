# Country IP Blocking | Windows Security Studio

<p align="center">
</p>

## Targeted Lists

The [Windows Security Studio App](https://github.com/OFFSECHQ/windows-security-studio/wiki/Windows-Security-Studio) uses the newest range of `IPv4` and `IPv6` addresses of [State Sponsors of Terrorism](https://www.state.gov/state-sponsors-of-terrorism/) and [OFAC Sanctioned Countries](https://orpa.princeton.edu/export-controls/sanctioned-countries), directly [from official IANA sources](https://github.com/HotCakeX/Official-IANA-IP-blocks) repository, then creates 2 rules (inbound and outbound) for each list in Windows firewall, completely blocking connections to and from those countries.

Once the firewall rules are active, enable and review [Windows Firewall logging](https://learn.microsoft.com/windows/security/operating-system-security/network-security/windows-firewall/configure-logging) to determine whether connections to or from those countries were blocked.

> [!NOTE]
> Threat actors can use VPN, VPS etc. to mask their originating IP address and location. So don't take this category as the perfect solution for network protection.

## Individual Country IP Blocking

<p align="center">
</p>

You can use this feature to block individual countries in Windows Firewall. Simply search for a country's name in the list and block/unblock all of its IPv4 and IPV6 ranges in just a few seconds.
