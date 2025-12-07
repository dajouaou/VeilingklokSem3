// src/components/ControlPanel.jsx

export default function ControlPanel({ start, next, close }) {
    return (
        <div className="vm-control-buttons">
            <button
                type="button"
                className="vm-btn vm-btn-primary"
                onClick={start}
            >
                ▶ Start veiling
            </button>

            <button
                type="button"
                className="vm-btn vm-btn-secondary"
                onClick={next}
            >
                ➜ Volgend product
            </button>

            <button
                type="button"
                className="vm-btn vm-btn-danger"
                onClick={close}
            >
                ✖ Huidig product sluiten
            </button>
        </div>
    );
}
