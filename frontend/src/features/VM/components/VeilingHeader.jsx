// src/features/VM/components/VeilingHeader.jsx

import { formatVeilingStatus } from "../utils/formatVeilingStatus";
import { formatDateTime } from "../utils/formatters";
// src/features/VM/components/VeilingHeader.jsx
import { formatVeilingStatus, formatDateTime } from "../utils/formatters";


export default function VeilingHeader({
                                          veilingNaam,
                                          locatie,
                                          vmNaam,
                                          status,
                                          startTijdUtc,
                                          eindTijdUtc,
                                      }) {
    const title = veilingNaam ?? "Veiling";
    const locationLabel = locatie ?? "Onbekende locatie";
    const vmLabel = vmNaam ?? "Onbekende veilingmeester";
    const statusLabel = formatVeilingStatus(status);
    const startLabel = startTijdUtc ? formatDateTime(startTijdUtc) : "—";
    const endLabel = eindTijdUtc ? formatDateTime(eindTijdUtc) : "—";

    return (
        <div className="vm-header">
            <div className="vm-header-main">
                <h1 className="vm-header-title">{title}</h1>
                <div className="vm-header-subtitle">{locationLabel}</div>
            </div>

            <div className="vm-header-meta">
                <div className="vm-header-row">
                    <span className="vm-header-label">Veilingmeester:</span>
                    <span className="vm-header-value">{vmLabel}</span>
                </div>
                <div className="vm-header-row">
                    <span className="vm-header-label">Status:</span>
                    <span className="vm-header-value">{statusLabel}</span>
                </div>
                <div className="vm-header-row">
                    <span className="vm-header-label">Start:</span>
                    <span className="vm-header-value">{startLabel}</span>
                </div>
                <div className="vm-header-row">
                    <span className="vm-header-label">Einde:</span>
                    <span className="vm-header-value">{endLabel}</span>
                </div>
            </div>
        </div>
    );
}
