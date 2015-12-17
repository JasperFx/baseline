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
    if which("dnvm").nil?
	  puts "dnvm not found, starting install"
	  if Gem.win_platform?
	    puts "installing dnvm for Windows OS"
		sh "@powershell -NoProfile -ExecutionPolicy unrestricted -Command \"&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}\""
	  else
	    puts "installing dnvm for *nix systems"
		sh "curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_BRANCH=dev sh && source ~/.dnx/dnvm/dnvm.sh"
	  end
	else
	  puts "dnvm found, skipping install"
	end
    sh "dnvm install 1.0.0-rc1-update1 -arch x64 -r clr"
	sh "dnvm use 1.0.0-rc1-update1 -arch x64 -r clr -p"
end

desc "Update the version information for the build"
task :version do
  file = File.read("src/Baseline/project.json", :encoding => 'bom|utf-8')
  project_hash = JSON.parse(file)
  project_version = project_hash["version"]
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
  sh "dnu restore"
end

desc 'Run the unit tests'
task :test => [:restore] do
	sh "dnx -p src/Baseline.Testing test"
end

desc "Pack up the nupkg file"
task :pack => [:restore] do
	Dir.mkdir "artifacts"
	sh "dnu pack src/Baseline --out ./artifacts --configuration #{COMPILE_TARGET}"
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

