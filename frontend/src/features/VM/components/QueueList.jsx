export default function QueueList({ queue }) {
    return (
        <div className="card">
            <h2>Queue</h2>

            {queue.length === 0 && <p>Geen producten in queue</p>}

            <ul>
                {queue.map(item => (
                    <li key={item.id}>
                        {item.product.naam} — {item.hoeveelheid} stuks
                    </li>
                ))}
            </ul>
        </div>
    );
}