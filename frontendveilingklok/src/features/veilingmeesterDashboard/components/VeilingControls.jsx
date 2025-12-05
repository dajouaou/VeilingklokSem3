export default function VeilingControls({
    veiling,
    onStart,
    onPause,
    onResume,
    onStop
}) {
    return (
        <div className="d-flex gap-2 my-3">

            {!veiling && (
                <button className="btn btn-success" onClick={onStart}>
                    Veiling starten
                </button>
            )}

            {veiling && !veiling.isPauze && (
                <button className="btn btn-warning" onClick={onPause}>
                    Pauzeren
                </button>
            )}

            {veiling && veiling.isPauze && (
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
