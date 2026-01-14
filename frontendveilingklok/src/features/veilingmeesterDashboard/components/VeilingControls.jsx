export default function VeilingControls({ veiling, onStart, onPause, onResume, onStop }) {
    const isPaused =
        !!veiling &&
        (veiling.isPauze === true ||
            veiling.isGepauzeerd === true ||
            veiling.status === "Gepauzeerd" ||
            veiling.status === 2);

    return (
        <div className="vm-actions">
            {!veiling && (
                <button type="button" className="vm-btn vm-btn-primary" onClick={() => onStart()}>
                    Veiling starten
                </button>
            )}

            {veiling && !isPaused && (
                <button type="button" className="vm-btn vm-btn-warn" onClick={onPause}>
                    Pauzeren
                </button>
            )}

            {veiling && isPaused && (
                <button type="button" className="vm-btn vm-btn-primary" onClick={onResume}>
                    Hervatten
                </button>
            )}

            {veiling && (
                <button type="button" className="vm-btn vm-btn-danger" onClick={onStop}>
                    Stoppen
                </button>
            )}
        </div>
    );
}
