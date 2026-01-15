import Navbar from "../../shared/components/Navbar";
import HeroBannerPreLogin from "../../shared/components/HeroBannerPreLogin";
import FeatureHighlights from "../../shared/components/FeatureHighlights";
import ReviewSectionPreLogin from "../../shared/components/ReviewSectionPreLogin";
import CTASectionPreLogin from "../../shared/components/CTASectionPreLogin";
import Footer from "../../shared/components/Footer";

export default function HomepagePreLogin() {
    return (
        <>
            <Navbar />

            <HeroBannerPreLogin />
            <FeatureHighlights />
            <ReviewSectionPreLogin />
            <CTASectionPreLogin />

            <Footer />
        </>
    );
}
