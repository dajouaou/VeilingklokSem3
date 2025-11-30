export default function ControlPanel({ actions }) {
    return (
        <div className="card">
            <h2>Controle</h2>
            <button onClick={actions.pause}>⏸ Pauzeer</button>
            <button onClick={actions.resume}>▶ Hervat</button>
            <button onClick={actions.stop}>⏹ Stop</button>
        </div>
    );
}
