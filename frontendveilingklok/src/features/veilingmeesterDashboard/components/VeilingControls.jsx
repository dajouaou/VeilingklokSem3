export default function VeilingControls({ veiling, onStart, onPause, onResume, onStop }) {
    // Backend/DTO kan status op meerdere manieren aanleveren:
    // - boolean flags (isPauze/isGepauzeerd)
    // - status string
    // - status nummer (2)
    // Daarom checken we meerdere velden zodat de UI altijd klopt.
    const isPaused =
        !!veiling &&
        (veiling.isPauze === true ||
            veiling.isGepauzeerd === true ||
            veiling.status === "Gepauzeerd" ||
            veiling.status === 2);

    return (
        <div className="vm-actions">
            {/* Als er nog geen actieve veiling is, kan je starten */}
            {!veiling && (
                <button type="button" className="vm-btn vm-btn-primary" onClick={() => onStart()}>
                    Veiling starten
                </button>
            )}

            {/* Als veiling actief en niet gepauzeerd: pauze knop */}
            {veiling && !isPaused && (
                <button type="button" className="vm-btn vm-btn-warn" onClick={onPause}>
                    Pauzeren
                </button>
            )}

            {/* Als veiling gepauzeerd: hervat knop */}
            {veiling && isPaused && (
                <button type="button" className="vm-btn vm-btn-primary" onClick={onResume}>
                    Hervatten
                </button>
            )}

            {/* Stoppen kan altijd zolang er een actieve veiling is */}
            {veiling && (
                <button type="button" className="vm-btn vm-btn-danger" onClick={onStop}>
                    Stoppen
                </button>
            )}
        </div>
    );
}
