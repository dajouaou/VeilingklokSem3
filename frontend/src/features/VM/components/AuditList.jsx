// src/features/VM/components/AuditList.jsx
import { formatDateTime } from "../utils/formatters";

export default function AuditList({ entries }) {
    if (!entries || entries.length === 0) {
        return null;
    }

    return (
        <ul className="vm-audit-list">
            {entries.map((entry) => {
                const action = entry.action ?? "Onbekende actie";
                const actor =
                    entry.actorNaam ?? entry.actor ?? "Onbekende gebruiker";
                const time = entry.createdAtUtc
                    ? formatDateTime(entry.createdAtUtc)
                    : null;

                return (
                    <li key={entry.id} className="vm-audit-item">
                        <div className="vm-audit-action">{action}</div>
                        <div className="vm-audit-meta">
                            {actor}
                            {time && (
                                <>
                                    {" "}
                                    •{" "}
                                    <span className="vm-audit-time">
                                        {time}
                                    </span>
                                </>
                            )}
                        </div>
                    </li>
                );
            })}
        </ul>
    );
}
