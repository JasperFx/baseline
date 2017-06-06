require 'json'

COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']

tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')

task :ci => [:version, :default, :pack]

task :default => [:test]

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
	FileUtils.rm_rf 'artifacts'
	FileUtils.rm_rf 'TestData'
end

task :install do
    if which("dotnet").nil?
	  puts "dotnet cli not found"
	  if Gem.win_platform?
      raise 'dotnet not found on Windows, must install sdk first'
	  else
	    puts "installing dotnet for *nix systems"
      sh 'curl -sSL https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0-preview1/scripts/obtain/dotnet-install.sh | bash /dev/stdin --version 1.0.0-preview1-002702 --install-dir ~/dotnet'
      sh 'sudo ln -s ~/dotnet/dotnet /usr/local/bin'
	  end
	else
	  puts 'dotnet cli found'
	end
end

desc "Update the version information for the build"
task :version do
  project_version = "1.4.0"
  build_number = project_version.gsub(/\*$/, build_revision) if project_version.include? "-*"
  build_number = "#{project_version}.#{build_revision}" unless project_version.include? "-*"
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  puts "##teamcity[buildNumber '#{build_number}']" unless tc_build_number.nil?
  puts "Version: #{build_number}" if tc_build_number.nil?
  
  options = {
	:trademark => commit,
	:informational_version => build_number
  }
  
  puts "Writing src/CommonAssemblyInfo.cs..."
	File.open('src/CommonAssemblyInfo.cs', 'w') do |file|
		file.write "using System.Reflection;\n"
		file.write "using System.Runtime.InteropServices;\n"
		file.write "[assembly: AssemblyTrademark(\"#{options[:trademark]}\")]\n"	
		file.write "[assembly: AssemblyInformationalVersion(\"#{options[:informational_version]}\")]\n"
	end
end

desc 'Restore the packages'
task :restore => [:clean, :install] do
  sh 'dotnet restore src/Baseline.sln'
end

desc 'Run the unit tests'
task :test => [:restore] do
	sh 'dotnet test src/Baseline.Testing/Baseline.Testing.csproj'
end

desc "Pack up the nupkg file"
task :pack => [:restore] do
	Dir.mkdir "artifacts"
  ENV["DOTNET_BUILD_VERSION"] = build_revision
	sh "dotnet pack src/Baseline/Baseline.csproj --output ./../../artifacts --configuration Release"
end


desc "Launches VS to the Marten solution file"
task :sln do
	sh "start src/Baseline.sln"
end

"Launches the documentation project in editable mode"
task :docs => [:restore] do
	sh "packages/Storyteller/tools/st.exe doc-run -v #{build_number}"
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

