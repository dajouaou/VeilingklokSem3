import { Link } from "react-router-dom";
import { useHome } from "../hooks/useHome";
import "../../../styles/HomePage.css";

export default function HomePage() {
    const { data, loading, error } = useHome();

    const appName = data?.appName ?? "Digitale Veilingklok";
    const headerLink = data?.header?.rightLink ?? { label: "Inloggen", href: "/login" };

    const hero = data?.hero ?? {};
    const kicker = hero.kicker ?? "Beveiligde toegang";
    const title = hero.title ?? "Digitale veilingomgeving voor de sierteelt";
    const subtitle = hero.subtitle ?? "Realtime, betrouwbaar en professioneel";
    const primary = hero.primaryAction ?? { label: "Inloggen", href: "/login" };
    const secondary = hero.secondaryAction ?? null;

    const roles = data?.roles ?? [];
    const footerLeft = data?.footer?.securityNote ?? "Beveiligde verbinding";
    const footerRight = data?.footer?.copyright ?? "© Digitale Veilingklok";

    if (loading) {
        return (
            <main className="home">
                <div className="homeShell">
                    <header className="homeHeader">
                        <div className="homeBrand">{appName}</div>
                        <div className="homeHeaderRight">
                            <span className="homePill">Laden…</span>
                        </div>
                    </header>

                    <section className="homeHero" aria-label="Start">
                        <div className="homeHeroCard">
                            <div className="homeKickerSkeleton" />
                            <div className="homeTitleSkeleton" />
                            <div className="homeSubtitleSkeleton" />
                            <div className="homeActionsSkeleton" />
                        </div>
                    </section>

                    <section className="homeRoles" aria-label="Rollen">
                        <div className="homeRoleCard homeRoleCard--skeleton" />
                        <div className="homeRoleCard homeRoleCard--skeleton" />
                        <div className="homeRoleCard homeRoleCard--skeleton" />
                    </section>

                    <footer className="homeFooter">
                        <span className="homeFooterLeft">{footerLeft}</span>
                        <span className="homeFooterRight">{footerRight}</span>
                    </footer>
                </div>
            </main>
        );
    }

    if (error) {
        return (
            <main className="home">
                <div className="homeShell">
                    <header className="homeHeader">
                        <div className="homeBrand">{appName}</div>
                        <div className="homeHeaderRight">
                            <Link className="homeLink" to="/login">
                                Inloggen
                            </Link>
                        </div>
                    </header>

                    <section className="homeHero" aria-label="Start">
                        <div className="homeHeroCard">
                            <p className="homeKicker">{kicker}</p>
                            <h1 className="homeTitle">Kan gegevens niet laden</h1>
                            <p className="homeSubtitle">{error}</p>

                            <div className="homeActions">
                                <Link className="homeBtnPrimary" to="/login">
                                    Inloggen
                                </Link>
                                <Link className="homeBtnSecondary" to="/">
                                    Opnieuw proberen
                                </Link>
                            </div>
                        </div>
                    </section>

                    <footer className="homeFooter">
                        <span className="homeFooterLeft">{footerLeft}</span>
                        <span className="homeFooterRight">{footerRight}</span>
                    </footer>
                </div>
            </main>
        );
    }

    return (
        <main className="home">
            <div className="homeShell">
                <header className="homeHeader">
                    <div className="homeBrand">{appName}</div>
                    <div className="homeHeaderRight">
                        <Link className="homeLink" to={headerLink.href}>
                            {headerLink.label}
                        </Link>
                    </div>
                </header>

                <section className="homeHero" aria-label="Start">
                    <div className="homeHeroCard">
                        <p className="homeKicker">{kicker}</p>
                        <h1 className="homeTitle">{title}</h1>
                        <p className="homeSubtitle">{subtitle}</p>

                        <div className="homeActions">
                            <Link className="homeBtnPrimary" to={primary.href}>
                                {primary.label}
                            </Link>

                            {secondary ? (
                                <Link className="homeBtnSecondary" to={secondary.href}>
                                    {secondary.label}
                                </Link>
                            ) : null}
                        </div>
                    </div>
                </section>

                <section className="homeRoles" aria-label="Rollen">
                    {roles.map((r) => (
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
                    <span className="homeFooterLeft">{footerLeft}</span>
                    <span className="homeFooterRight">{footerRight}</span>
                </footer>
            </div>
        </main>
    );
}
