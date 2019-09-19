module.exports = function(grunt) {
    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        sass: {
            dev: {
                options: {
                    sourcemap: 'file',
                    style: 'compressed'
                },
                files: {
                    "css/style.css": "scss/style.scss" // destination file and source file
                }
            }
        },
        concat: {
            development: {
                src: [
                    'css/plugins.min.css',
                    'css/addons.min.css',
                    'css/style.css',
                ],
                dest: 'css/main.css'
            },
            js_website: {
                src: [
                    'js/plugins.min.js',
                    'js/customize.js',
                    'js/canhcam.js',
                ],
                dest: 'js/app.js'
            }
        },
        cssmin: {
            development: {
                src: 'css/main.css',
                dest: 'css/main.min.css'
            }
        },
        uglify: {
            development: {
                src: 'js/app.js',
                dest: 'js/app.min.js',
            }
        },
        watch: {
            styles: {
                files: ['scss/**/*.scss', 'js/*.js'], // which files to watch
                tasks: ['sass:dev', 'concat', 'cssmin', 'uglify'],
                options: {
                    spawn: false,
                }
            }
        },
        /*jshint: {
            all: ['js/*.js']
        },*/
    });

    grunt.loadNpmTasks("grunt-contrib-sass");
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks("grunt-contrib-watch");

    // the default task can be run just by typing "grunt" on the command line
    grunt.registerTask('default', ['sass', 'concat', 'cssmin', 'uglify']);
};
