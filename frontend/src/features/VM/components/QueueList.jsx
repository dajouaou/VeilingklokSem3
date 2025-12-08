// src/features/VM/components/QueueList.jsx
import { formatQueueStatus } from "../utils/formatters";


export default function QueueList({ queue }) {
    if (!queue || queue.length === 0) {
        return null;
    }

    return (
        <ul className="vm-queue-list">
            {queue.map((item) => {
                const statusLabel =
                    item.status !== undefined && item.status !== null
                        ? formatQueueStatus(item.status)
                        : null;

                const productNaam = item.productNaam ?? "Onbekend product";
                const aanvoerder = item.aanvoerder ?? "Onbekende aanvoerder";
                const volgorde =
                    item.volgorde !== undefined && item.volgorde !== null
                        ? item.volgorde
                        : "—";

                return (
                    <li key={item.id} className="vm-queue-item">
                        <div className="vm-queue-name">{productNaam}</div>
                        <div className="vm-queue-meta">
                            <span>{aanvoerder}</span>
                            <span> • </span>
                            <span>Volgorde {volgorde}</span>
                            {statusLabel && (
                                <>
                                    <span> • </span>
                                    <span>{statusLabel}</span>
                                </>
                            )}
                        </div>
                    </li>
                );
            })}
        </ul>
    );
}
