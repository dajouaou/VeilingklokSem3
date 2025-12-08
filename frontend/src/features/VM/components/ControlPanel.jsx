// src/features/VM/components/ControlPanel.jsx
export default function ControlPanel({
                                         start,
                                         next,
                                         close,
                                         disabled = false,
                                         canStart = true,
                                         canNext = true,
                                         canClose = true,
                                     }) {
    const startDisabled = disabled || !canStart;
    const nextDisabled = disabled || !canNext;
    const closeDisabled = disabled || !canClose;

    return (
        <div className="vm-control-buttons">
            <button
                type="button"
                className="vm-btn vm-btn-primary"
                onClick={start}
                disabled={startDisabled}
            >
                ▶ Start veiling
            </button>

            <button
                type="button"
                className="vm-btn vm-btn-secondary"
                onClick={next}
                disabled={nextDisabled}
            >
                ➜ Volgend product
            </button>

            <button
                type="button"
                className="vm-btn vm-btn-danger"
                onClick={close}
                disabled={closeDisabled}
            >
                ✖ Huidig product sluiten
            </button>
        </div>
    );
}
