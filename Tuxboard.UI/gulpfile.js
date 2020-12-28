/// <binding BeforeBuild='default' />
var path = require('path'),
    gulp = require('gulp'),
    gp_clean = require('gulp-clean'),
    gp_concat = require('gulp-concat'),
    gp_sass = require('gulp-sass'),
    gp_sourcemaps = require('gulp-sourcemaps'),
    gp_typescript = require('gulp-typescript'),
    gp_uglify = require('gulp-uglify'),
    webpack = require("webpack-stream"),
    launch = require('./Properties/launchSettings.json');

var basePath = path.resolve(__dirname, "wwwroot");

var projectName = "tuxboard";

var srcPaths = {
    src: [
        path.resolve(basePath, 'src/'+projectName+'.ts'),
        path.resolve(basePath, 'src/**/*.ts')
    ],
    js: [path.resolve(basePath, 'src/js/**/*.js')],
    sass: [path.resolve(basePath, 'scss/' + projectName +'.scss')]
};

var destPaths = {
    css: path.resolve(basePath, 'css'),
    js: path.resolve(basePath, 'js')
};

var environment = {
    development: "development",
    production: "production",

    current: function () {
        return (process.env.ASPNETCORE_ENVIRONMENT ||
            (launch && launch.profiles['IIS Express'].environmentVariables.ASPNETCORE_ENVIRONMENT) ||
            this.development).toLowerCase();
    },
    isDevelopment: function () { return this.current() === this.development; },
    isProduction: function () { return this.current() === this.production; },
    getConfig: function() {
        if (this.isDevelopment()) {
            return require("./webpack.dev.js");
        }
        return require("./webpack.prod.js");
    }
};

gulp.task('testTask', function (done) {
    console.log('hello!');
    done();
});

/* Webpack */

gulp.task('webpack_clean', function () {
    return gulp.src(destPaths.js + "/*", { read: false })
        .pipe(gp_clean({ force: true }));
});

gulp.task('webpack', function () {
    return gulp.src(srcPaths.src)
        .pipe(webpack(environment.getConfig()))
        .pipe(gulp.dest(destPaths.js+"/"));
});

/* TypeScript */
gulp.task('ts_clean', function () {
    return gulp.src(destPaths.js + "/*", { read: false })
        .pipe(gp_clean({ force: true }));
});

gulp.task('ts', gulp.series(['ts_clean']), function() {
    return gulp.src(srcPaths.src)
        .pipe(gp_sourcemaps.init())
        .pipe(gp_typescript(require('./tsconfig.json').compilerOptions))
        .pipe(gp_uglify({ mangle: false }))
        .pipe(gp_sourcemaps.write('wwwroot/src'))
        .on('error',
            function(err) {
                console.error('Error!', err.message);
            })
        .pipe(gulp.dest(destPaths.js + "/"));
});

/* JavaScript */
gulp.task('js', function () {
    return gulp.src(srcPaths.js)
        .pipe(gp_uglify({ mangle: false })) // disable uglify
        .pipe(gp_concat(projectName + '.min.js')) // disable concat
        .on('error',
            function(err) {
                console.error('Error!', err.message);
            })
        .pipe(gulp.dest(destPaths.js));
});

gulp.task('js_clean', function (){
    return gulp.src(destPaths.js + "*", { read: false })
        .pipe(gp_clean({ force: true }));
});

/* SASS/CSS */
gulp.task('sass_clean', function () {
    return gulp.src(destPaths.css + "*.*", { read: false })
        .pipe(gp_clean({ force: true }));
});

gulp.task('sass', function() {
    return gulp.src(srcPaths.sass)
        .pipe(gp_sass())
        .pipe(gulp.dest(destPaths.css));
});

/* Defaults */

gulp.task('cleanup', gulp.series(['ts_clean', 'sass_clean']));

gulp.task('default', gulp.series(['webpack', 'sass']));

