/// <binding BeforeBuild='default' />
var path = require('path'),
    gulp = require('gulp'),
    gp_clean = require('gulp-clean'),
    sourcemaps = require('gulp-sourcemaps'),
    uglify = require("gulp-uglify"),
    buffer = require('vinyl-buffer'),
    source = require('vinyl-source-stream'),
    rename = require("gulp-rename"),
    gp_sass = require('gulp-sass'),
    browserify = require("browserify"),
    ts = require("gulp-typescript");

    var basePath = path.resolve(__dirname, "wwwroot");

var tsProject = ts.createProject('tsconfig.json');

var projectName = "tuxboard";

var srcPaths = {
    srcJs: [
        path.resolve(basePath, 'src/'+projectName+'.js')
    ],
    src: [
        path.resolve(basePath, 'src/'+projectName+'.ts'),
    ],
    js: [path.resolve(basePath, 'src/**/*.js')],
    sass: [path.resolve(basePath, 'scss/' + projectName +'.scss')]
};

var destPaths = {
    css: path.resolve(basePath, 'css'),
    js: path.resolve(basePath, 'js')
};

gulp.task('testTask', done => {
    console.log('hello!');
    done();
});

/* TypeScript */
gulp.task("ts", done => {
    return tsProject.src()
        .pipe(tsProject())
        .pipe(gulp.dest(path.resolve(basePath, 'src')));
});

gulp.task("ts_clean", done => {
    return gulp.src(srcPaths.srcJs)
        .pipe(gp_clean({ force: true }));
});

/* JavaScript */
gulp.task('js', done => {

    srcPaths.js.forEach(file => {

        const b = browserify({
            entries: file, // Only need initial file, browserify finds the deps
            debug: true, // Enable sourcemaps
            transform: [['babelify', { 'presets': ["es2015"] }]]
        });

        b.bundle()
            .pipe(source(path.basename(file)))
            .pipe(rename(path => {
                path.basename += ".min";
                path.extname = ".js";
            }))
            .pipe(buffer())
            .pipe(sourcemaps.init({ loadMaps: true }))
            .pipe(uglify())
            .pipe(sourcemaps.write())
            .pipe(gulp.dest(destPaths.js));

        done();
    });

});
gulp.task('js_clean', done => {
    return gulp.src(path.resolve(destPaths.js, '**/*.js'), { read: false })
        .pipe(gp_clean({ force: true }));
});

/* SASS/CSS */
gulp.task('sass_clean', done => {
    return gulp.src(destPaths.css + "*.*", { read: false })
        .pipe(gp_clean({ force: true }));
});

gulp.task('sass', done => {
    return gulp.src(srcPaths.sass)
        .pipe(gp_sass({ outputStyle: 'compressed' }))
        .pipe(rename({
            suffix: '.min'
        }))
        .pipe(gulp.dest(destPaths.css));
});

/* Defaults */
gulp.task('cleanup', gulp.series(['ts_clean', 'js_clean', 'sass_clean']));

gulp.task('default', gulp.series(['ts', 'js', 'sass']));


