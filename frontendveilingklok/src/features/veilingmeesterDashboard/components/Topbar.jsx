export default function Topbar({ title, veiling, onMenuClick }) {
    return (
        <header className="vm-topbar">
            <button className="vm-hamburger" onClick={onMenuClick}>
                Menu
            </button>

            <div>
                <h1>{title}</h1>
                <p>Beheer de klok, wachtrij en biedingen.</p>
            </div>

            {veiling && (
                <div className="vm-topbar-pill">
                    <span className="dot" />
                    Veiling #{veiling.id}
                </div>
            )}
        </header>
    );
}
