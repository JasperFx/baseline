require 'json'

BUILD_VERSION = '2.0.0'

tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')

task :ci => [:default, :pack]

task :default => [:test]

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
	FileUtils.rm_rf 'artifacts'
	FileUtils.rm_rf 'TestData'
end




desc 'Run the unit tests'
task :test do
	sh 'dotnet test src/Baseline.Testing/Baseline.Testing.csproj'
end

desc "Pack up the nupkg file"
task :pack => [:restore] do
	Dir.mkdir "artifacts"
  ENV["DOTNET_BUILD_VERSION"] = build_revision
	sh "dotnet pack src/Baseline/Baseline.csproj --output ./artifacts --configuration Release"
end


desc "Launches VS to the Baseline solution file"
task :sln do
	sh "start src/Baseline.sln"
end

"Launches the documentation project in editable mode"
task :docs do
    sh "dotnet restore docs.csproj"
	sh "dotnet stdocs run -v #{BUILD_VERSION}"
end

"Exports the documentation to jasperfx.github.io/baseline - requires Git access to that repo though!"
task :publish do
	FileUtils.remove_dir('doc-target') if Dir.exists?('doc-target')

	if !Dir.exists? 'doc-target' 
		Dir.mkdir 'doc-target'
		sh "git clone -b gh-pages https://github.com/jasperfx/baseline.git doc-target"
	else
		Dir.chdir "doc-target" do
			sh "git checkout --force"
			sh "git clean -xfd"
			sh "git pull origin master"
		end
	end
	
	sh "dotnet restore"
	sh "dotnet stdocs export doc-target ProjectWebsite --version #{BUILD_VERSION} --project baseline"
	
	Dir.chdir "doc-target" do
		sh "git add --all"
		sh "git commit -a -m \"Documentation Update for #{BUILD_VERSION}\" --allow-empty"
		sh "git push origin gh-pages"
	end
	

	

end

def which(cmd)
  exts = ENV['PATHEXT'] ? ENV['PATHEXT'].split(';') : ['']
  ENV['PATH'].split(File::PATH_SEPARATOR).each do |path|
    exts.each { |ext|
      exe = File.join(path, "#{cmd}#{ext}")
      return exe if File.executable?(exe) && !File.directory?(exe)
    }
  end
  return nil
end

