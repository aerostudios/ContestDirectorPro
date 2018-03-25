import { Aurelia, PLATFORM, Container } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import * as signalR from "@aspnet/signalr-client"

export class App {
    router?: Router;
    
    configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'CDP.ContestHost.Site';
        config.map([{
            route: [ '', 'home' ],
            name: 'home',
            settings: { icon: 'home' },
            moduleId: PLATFORM.moduleName('../../features/pages/home/homePage'),
            nav: true,
            title: 'Home'
        }, {
            route: 'contestPage',
            name: 'contestPage',
            settings: { icon: 'cloud-download' },
            moduleId: PLATFORM.moduleName('../../features/pages/contest/contestPage'),
            nav: true,
            title: 'Contest Connect'
        }, {
            route: 'scoreboardPage',
            name: 'scoreboardPage',
            settings: { icon: 'eye-open' },
            moduleId: PLATFORM.moduleName('../../features/pages/scoreboard/scoreboardPage'),
            nav: true,
            title: 'Scoreboard'
        }, {
            route: 'timerRegistrationPage',
            name: 'timerRegistrationPage',
            settings: { icon: 'barcode' },
            moduleId: PLATFORM.moduleName('../../features/pages/timerRegistration/timerRegistrationPage'),
            nav: true,
            title: 'Timer Registration'
        }, {
            route: 'timingPage',
            name: 'timingPage',
            settings: { icon: 'pencil' },
            moduleId: PLATFORM.moduleName('../../features/pages/timing/timingPage'),
            nav: true,
            title: 'Timing'
        }, {
            route: 'faqPage',
            name: 'faqPage',
            settings: { icon: 'asterisk' },
            moduleId: PLATFORM.moduleName('../../features/pages/faq/faqPage'),
            nav: true,
            title: 'FAQ'
        }, {
            route: 'aboutPage',
            name: 'aboutPage',
            settings: { icon: 'asterisk' },
            moduleId: PLATFORM.moduleName('../../features/pages/about/aboutPage'),
            nav: true,
            title: 'About'
        }]);

        this.router = router;
    }
}
