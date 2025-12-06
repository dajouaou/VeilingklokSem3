export default function ControlPanel({ start, next, close }) {
    return (
        <div className="card" style={{ marginBottom: "20px" }}>
            <h2>Besturing</h2>

            <button onClick={start} style={btnStyle}>
                ▶ Start veiling
            </button>

            <button onClick={next} style={btnStyle}>
                ➜ Volgend product
            </button>

            <button onClick={close} style={btnStyle}>
                ✖ Huidig product sluiten
            </button>
        </div>
    );
}

const btnStyle = {
    display: "block",
    margin: "10px 0",
    padding: "10px",
    fontSize: "16px",
    cursor: "pointer",
};