// src/features/veiling/components/BidList.jsx
export default function BidList({ bids, currentUserId }) {
    if (!bids || bids.length === 0) {
        return <p>Er zijn nog geen biedingen geplaatst.</p>;
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
                maxHeight: "260px",
                overflowY: "auto",
                border: "1px solid #eee",
                borderRadius: "8px",
                paddingInline: "0.5rem",
                paddingBlock: "0.5rem",
            }}
        >
            {bids.map((bid) => {
                const isMe =
                    currentUserId &&
                    bid.koperId &&
                    Number(currentUserId) === Number(bid.koperId);

                const date = bid.placedAtUtc
                    ? new Date(bid.placedAtUtc)
                    : null;

                const timeLabel = date
                    ? date.toLocaleTimeString("nl-NL", { hour: "2-digit", minute: "2-digit", second: "2-digit" })
                    : "";

                return (
                    <li
                        key={bid.id ?? `${bid.koperId}-${bid.placedAtUtc}`}
                        style={{
                            borderBottom: "1px solid #f0f0f0",
                            padding: "0.35rem 0.25rem",
                            display: "flex",
                            justifyContent: "space-between",
                            fontSize: "0.9rem",
                        }}
                    >
                        <div>
                            <strong>€ {Number(bid.amount || 0).toFixed(2)}</strong>{" "}
                            <span style={{ color: "#555" }}>
                                door {bid.koperNaam || `koper #${bid.koperId ?? "?"}`}
                                {isMe && " (jij)"}
                            </span>
                        </div>
                        <div style={{ color: "#999", fontSize: "0.8rem" }}>{timeLabel}</div>
                    </li>
                );
            })}
        </ul>
    );
}
