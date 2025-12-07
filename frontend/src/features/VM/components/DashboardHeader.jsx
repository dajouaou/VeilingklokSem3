// src/components/DashboardHeader.jsx

export default function DashboardHeader({ title, subtitle, userLabel }) {
    return (
        <header className="vm-header card">
            <div className="vm-header-left">
                <span className="vm-logo-dot" />
                <span className="vm-logo-text">Veilingklok</span>
            </div>

            <div className="vm-header-center">
                <div className="vm-header-title">{title}</div>
                <div className="vm-header-subtitle">{subtitle}</div>
            </div>

            <div className="vm-header-right">
                <span className="vm-header-user">{userLabel}</span>
            </div>
        </header>
    );
}
