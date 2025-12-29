function isStatus(s, v) {
    return String(s) === v;
}

export function ControlPanel({ status, current, actions }) {
    const scheduled = isStatus(status, "Scheduled");
    const running = isStatus(status, "Running");
    const paused = isStatus(status, "Paused");
    const finished = isStatus(status, "Finished");

    const currentActive = current && String(current.status) === "Active";

    const canStart = scheduled;
    const canPause = running;
    const canResume = paused;
    const canStop = !finished;
    const canNext = running;
    const canClose = running && currentActive;
    const canReset = scheduled || finished;

    return (
        <div className="vm-panel">
            <div className="vm-panel__header">
                <div className="vm-panel__title">Besturing</div>
                <div className="vm-panel__hint">Acties zijn direct gekoppeld aan de backend</div>
            </div>

            <div className="vm-controls">
                <div className="vm-controls__group">
                    <div className="vm-controls__label">Veiling</div>
                    <div className="vm-controls__buttons">
                        <button className="vm-btn" disabled={!canStart} onClick={actions.start} type="button">
                            Start veiling
                        </button>
                        <button className="vm-btn" disabled={!canPause} onClick={actions.pause} type="button">
                            Pauzeer
                        </button>
                        <button className="vm-btn" disabled={!canResume} onClick={actions.resume} type="button">
                            Hervat
                        </button>
                        <button
                            className="vm-btn vm-btn--danger"
                            disabled={!canStop}
                            onClick={() => {
                                const ok = window.confirm("Weet je zeker dat je de veiling wil stoppen?");
                                if (ok) actions.stop();
                            }}
                            type="button"
                        >
                            Stop veiling
                        </button>
                    </div>
                </div>

                <div className="vm-controls__group">
                    <div className="vm-controls__label">Product</div>
                    <div className="vm-controls__buttons">
                        <button
                            className="vm-btn"
                            disabled={!canNext}
                            onClick={actions.next}
                            type="button"
                            title="Volgende product (huidige overslaan)"
                        >
                            Volgende
                        </button>

                        <button
                            className="vm-btn vm-btn--accent"
                            disabled={!canClose}
                            onClick={actions.closeCurrent}
                            type="button"
                        >
                            Sluit huidig (verkocht)
                        </button>

                        <button
                            className="vm-btn"
                            disabled={!current || !current.id || finished}
                            onClick={() => current?.id && actions.skip(current.id)}
                            type="button"
                            title="Overslaan van huidig product"
                        >
                            Overslaan
                        </button>

                        <button
                            className="vm-btn vm-btn--danger"
                            disabled={!canReset}
                            onClick={() => {
                                const ok = window.confirm("Reset zet alles terug naar startprijs en queued. Doorgaan?");
                                if (ok) actions.reset();
                            }}
                            type="button"
                        >
                            Reset veiling
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
