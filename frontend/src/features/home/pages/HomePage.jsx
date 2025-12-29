import { Link } from "react-router-dom";
import { useHome } from "../hooks/useHome";
import "../../../styles/HomePage.css";



export default function HomePage() {
    const { data, loading, error } = useHome();

    if (loading) {
        return (
            <main className="home">
                <div className="homeShell">
                    <header className="homeHeader">
                        <div className="homeBrand">Digitale Veilingklok</div>
                        <div className="homeHeaderRight">
                            <span className="homePill">Laden…</span>
                        </div>
                    </header>
                    <section className="homeCard">
                        <div className="homeTitleSkeleton" />
                        <div className="homeSubtitleSkeleton" />
                        <div className="homeActionsSkeleton" />
                    </section>
                </div>
            </main>
        );
    }

    if (error) {
        return (
            <main className="home">
                <div className="homeShell">
                    <header className="homeHeader">
                        <div className="homeBrand">Digitale Veilingklok</div>
                        <div className="homeHeaderRight">
                            <Link className="homeLink" to="/login">Inloggen</Link>
                        </div>
                    </header>

                    <section className="homeCard">
                        <h1 className="homeTitle">Er ging iets mis</h1>
                        <p className="homeSubtitle">{error}</p>
                        <div className="homeActions">
                            <Link className="homeBtnPrimary" to="/login">Inloggen</Link>
                        </div>
                    </section>

                    <footer className="homeFooter">
                        <span className="homeFooterLeft">Beveiligde verbinding</span>
                        <span className="homeFooterRight">© Digitale Veilingklok</span>
                    </footer>
                </div>
            </main>
        );
    }

    return (
        <main className="home">
            <div className="homeShell">
                <header className="homeHeader">
                    <div className="homeBrand">{data?.appName ?? "Digitale Veilingklok"}</div>
                    <div className="homeHeaderRight">
                        <Link className="homeLink" to={data?.primaryAction?.href ?? "/login"}>
                            {data?.primaryAction?.label ?? "Inloggen"}
                        </Link>
                    </div>
                </header>

                <section className="homeCard" aria-label="Start">
                    <h1 className="homeTitle">{data?.title ?? ""}</h1>
                    <p className="homeSubtitle">{data?.subtitle ?? ""}</p>

                    <div className="homeActions">
                        <Link className="homeBtnPrimary" to={data?.primaryAction?.href ?? "/login"}>
                            {data?.primaryAction?.label ?? "Inloggen"}
                        </Link>

                        {data?.secondaryAction ? (
                            <Link className="homeBtnSecondary" to={data.secondaryAction.href}>
                                {data.secondaryAction.label}
                            </Link>
                        ) : null}
                    </div>
                </section>

                <section className="homeRoles" aria-label="Rollen">
                    {(data?.roles ?? []).map((r) => (
                        <div key={r.roleName} className="homeRoleCard">
                            <div className="homeRoleTop">
                                <span className="homeRoleDot" aria-hidden="true" />
                                <h2 className="homeRoleTitle">{r.roleName}</h2>
                            </div>
                            <p className="homeRoleText">{r.description}</p>
                        </div>
                    ))}
                </section>

                <footer className="homeFooter">
                    <span className="homeFooterLeft">{data?.footer?.securityNote ?? "Beveiligde verbinding"}</span>
                    <span className="homeFooterRight">{data?.footer?.copyright ?? "© Digitale Veilingklok"}</span>
                </footer>
            </div>
        </main>
    );
}
