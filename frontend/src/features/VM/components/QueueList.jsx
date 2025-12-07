// src/components/QueueList.jsx

export default function QueueList({ queue }) {
    if (!queue || queue.length === 0) {
        
        return null;
    }

    return (
        <ul className="vm-queue-list">
            {queue.map((item) => (
                <li key={item.id} className="vm-queue-item">
                    <div className="vm-queue-name">
                        {item.productNaam}
                    </div>
                    <div className="vm-queue-meta">
                        <span>{item.aanvoerder}</span>
                        <span>•</span>
                        <span>Volgorde {item.volgorde}</span>
                    </div>
                </li>
            ))}
        </ul>
    );
}
