(function () {
    'use strict';

    angular
        .module('NetworkManager')
        .factory('DirectoryService', DirectoryService);

    DirectoryService.$inject = ['$resource', '$q', '$http'];

    function DirectoryService($resource, $q, $http) {
        var resource = $resource('/api/Directories/:action/:param', { action: '@action', param: '@param' }, {
            'update': { method: 'PUT' }
        });

        var _getDirectoriesInfo = function (path) {
            showLoadingAnim();
            var deferred = $q.defer();
            $http.post('/api/Directories/getDirectoriesInfo', path)
                .then(function (result) {
                    hideLoadingAnim();
                    deferred.resolve(result);
                },
				function (response) {
				    hideLoadingAnim();
				    deferred.reject(response);
				});
            return deferred.promise;

        };

        var _getDirectory = function (path, pathStr) {
            showLoadingAnim();
            var deferred = $q.defer();
            $http.post('/api/Directories/getDirectory', path, pathStr)
				.then(function (result) {
				    hideLoadingAnim();
				    if (result == null) {
				        result = [];
				    };
				    deferred.resolve(result);
				},
				function (response) {
				    hideLoadingAnim();
				    deferred.reject(response);
				});
            return deferred.promise;
        };

        var _checkDir = function (dir) {
            showLoadingAnim();
            var deferred = $q.defer();
            $http.post('/api/Directories/checkDir', dir)
                .then(function (result) {
                    hideLoadingAnim();
                    deferred.resolve(result);
                },
                        function (response) {
                            hideLoadingAnim();
                            deferred.reject(response);
                        });
            return deferred.promise;
        };
        var _openRoot = function (dir) {
            showLoadingAnim();
            var deferred = $q.defer();
            $http.post('/api/Directories/openRoot', dir)
                .then(function (result) {
                    hideLoadingAnim();
                    deferred.resolve(result);
                },
                        function (response) {
                            hideLoadingAnim();
                            deferred.reject(response);
                        });
            return deferred.promise;
        };
        var _openParrent = function (dir) {
            showLoadingAnim();
            var deferred = $q.defer();
            $http.post('/api/Directories/openParrent', dir)
                .then(function (result) {
                    hideLoadingAnim();
                    deferred.resolve(result);
                },
                        function (response) {
                            hideLoadingAnim();
                            deferred.reject(response);
                        });
            return deferred.promise;
        };

        return {
            getDirectoriesInfo: _getDirectoriesInfo,
            getDirectory: _getDirectory,
            checkDir: _checkDir,
            openRoot: _openRoot,
            openParrent: _openParrent,
        };

    }
    function hideLoadingAnim()
    {
        $("#preloader").hide();
        $("#filemanager").show();
    }
    function showLoadingAnim()
    {
        $("#filemanager").hide();
        $("#preloader").show();
    }

})();