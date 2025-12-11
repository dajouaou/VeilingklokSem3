// src/features/veiling/components/ClockDisplay.jsx
function formatMsToSeconds(ms) {
    if (!ms || ms < 0) return "0";
    return Math.ceil(ms / 1000).toString();
}

export default function ClockDisplay({ timeLeftMs, currentPrice }) {
    const seconds = formatMsToSeconds(timeLeftMs);

    return (
        <div
            style={{
                border: "1px solid #ddd",
                borderRadius: "8px",
                padding: "0.75rem 1rem",
                marginBottom: "1rem",
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
            }}
        >
            <div>
                <div style={{ fontSize: "0.85rem", color: "#666" }}>Tijd over</div>
                <div style={{ fontSize: "1.5rem", fontWeight: "bold" }}>{seconds} s</div>
            </div>
            <div style={{ textAlign: "right" }}>
                <div style={{ fontSize: "0.85rem", color: "#666" }}>Huidige prijs</div>
                <div style={{ fontSize: "1.5rem", fontWeight: "bold" }}>
                    € {Number(currentPrice || 0).toFixed(2)}
                </div>
            </div>
        </div>
    );
}
