export default function VeilingControls({ veiling, onStart, onPause, onResume, onStop }) {
    const isPaused =
        !!veiling &&
        (
            veiling.isPauze === true ||
            veiling.isGepauzeerd === true ||
            veiling.status === "Gepauzeerd" ||
            veiling.status === 2 // als status enum als number binnenkomt
        );

    return (
        <div className="d-flex gap-2 my-3">
            {!veiling && (
                <button className="btn btn-success" onClick={() => onStart()}>
                    Veiling starten
                </button>
            )}

            {veiling && !isPaused && (
                <button className="btn btn-warning" onClick={onPause}>
                    Pauzeren
                </button>
            )}

            {veiling && isPaused && (
                <button className="btn btn-primary" onClick={onResume}>
                    Hervatten
                </button>
            )}

            {veiling && (
                <button className="btn btn-danger" onClick={onStop}>
                    Stoppen
                </button>
            )}
        </div>
    );
}
