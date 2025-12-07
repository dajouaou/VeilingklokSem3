// src/components/AuditList.jsx

export default function AuditList({ entries }) {
    if (!entries || entries.length === 0) {
        return null;
    }

    return (
        <ul className="vm-audit-list">
            {entries.map((entry) => (
                <li key={entry.id} className="vm-audit-item">
                    <div className="vm-audit-action">
                        {entry.action ?? "Onbekende actie"}
                    </div>
                    <div className="vm-audit-meta">
                        {entry.actorNaam ?? entry.actor ?? "Onbekende gebruiker"}
                        {entry.createdAtUtc && (
                            <>
                                {" "}
                                •{" "}
                                <span className="vm-audit-time">
                  {new Date(entry.createdAtUtc).toLocaleString("nl-NL")}
                </span>
                            </>
                        )}
                    </div>
                </li>
            ))}
        </ul>
    );
}
