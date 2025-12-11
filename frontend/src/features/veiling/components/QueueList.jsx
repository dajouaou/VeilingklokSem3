// src/features/veiling/components/QueueList.jsx
export default function QueueList({ queue }) {
    if (!queue || queue.length === 0) {
        return <p>Er staan geen producten meer in de rij.</p>;
    }

    return (
        <ul
            style={{
                listStyle: "none",
                padding: 0,
                margin: 0,
                display: "flex",
                flexDirection: "column",
                gap: "0.5rem",
            }}
        >
            {queue.map((item) => {
                const naam = item.product?.naam ?? "Onbekend product";
                return (
                    <li
                        key={item.id}
                        style={{
                            border: "1px solid #eee",
                            borderRadius: "6px",
                            padding: "0.5rem 0.75rem",
                            display: "flex",
                            justifyContent: "space-between",
                        }}
                    >
                        <div>
                            <strong>{naam}</strong>
                            <div style={{ fontSize: "0.85rem", color: "#666" }}>
                                Volgorde: {item.volgorde} · Hoeveelheid: {item.hoeveelheid}
                            </div>
                        </div>
                        <div style={{ textAlign: "right", fontSize: "0.9rem" }}>
                            <div>Status: {item.status}</div>
                            <div>Start: € {Number(item.startPrijs || 0).toFixed(2)}</div>
                        </div>
                    </li>
                );
            })}
        </ul>
    );
}
