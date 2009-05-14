<?xml version="1.0" encoding="ISO-8859-1"?>
<!-- ================================================================================ -->
<!-- Original source: -->
<!-- http://www.codeproject.com/KB/XML/XMLDocStylesheet/XMLDocStylesheet_src.zip -->
<!-- http://www.codeproject.com/KB/XML/XMLDocStylesheet.aspx -->
<!-- Amend, distribute, spindle and mutilate as desired, but don't remove this header -->
<!-- A simple XML Documentation to basic HTML transformation stylesheet -->
<!-- (c)2005 by Emma Burrows -->
<!-- ================================================================================ -->
<!-- Modifications copyright 2009 Len Popp -->
<!-- Permission is hereby granted, free of charge, to any person obtaining a copy -->
<!-- of this software and associated documentation files (the "Software"), to deal -->
<!-- in the Software without restriction, including without limitation the rights -->
<!-- to use, copy, modify, merge, publish, distribute, sublicense, and/or sell -->
<!-- copies of the Software, and to permit persons to whom the Software is -->
<!-- furnished to do so, subject to the following conditions: -->
<!-- -->
<!-- The above copyright notice and this permission notice shall be included in -->
<!-- all copies or substantial portions of the Software. -->
<!-- -->
<!-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR -->
<!-- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, -->
<!-- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE -->
<!-- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER -->
<!-- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, -->
<!-- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN -->
<!-- THE SOFTWARE. -->
<!-- ================================================================================ -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- DOCUMENT TEMPLATE -->
<!-- Format the whole document as a valid HTML document -->
<xsl:template match="/">
<html>
<head>
<link rel="stylesheet" type="text/css" href="xmldoc.css"/>
</head>
<body>
  <xsl:apply-templates select="//assembly">
    <xsl:sort select="@name" order="ascending"/>
  </xsl:apply-templates>
</body>
</html>
</xsl:template>

<!-- ASSEMBLY TEMPLATE -->
<!-- For each Assembly, display its name and then its member types -->
<xsl:template match="assembly">
<h1>Assembly: <xsl:value-of select="name"/></h1>
  <xsl:apply-templates select="//member[contains(@name,'T:')]">
    <xsl:sort select="@name" order="ascending"/>
  </xsl:apply-templates>
</xsl:template>

<!-- TYPE TEMPLATE -->
<!-- Loop through member types and display their properties and methods -->
<xsl:template match="//member[contains(@name,'T:')]">

  <!-- Two variables to make code easier to read -->
  <!-- A variable for the name of this type -->
  <xsl:variable name="MemberName"
                 select="substring-after(@name, '.')"/>

  <!-- Get the type's fully qualified name without the T: prefix -->
  <xsl:variable name="FullMemberName"
                 select="substring-after(@name, ':')"/>

  <!-- Display the type's name and information -->
  <h2 id="{@name}">Type: <xsl:value-of select="$MemberName"/></h2>
  <div class="contents">
  <xsl:apply-templates/>

  <!-- If this type has public fields, display them -->
  <xsl:if test="//member[contains(@name,concat('F:',$FullMemberName))]">
    <h3>Fields</h3>
    <div class="contents">
      <xsl:for-each select="//member[contains(@name,concat('F:',$FullMemberName,'.'))]">
        <p class="name" id="{@name}">
          <xsl:value-of select="substring-after(@name, concat('F:',$FullMemberName,'.'))"/>
        </p>
        <xsl:apply-templates/>
      </xsl:for-each>
    </div>
  </xsl:if>

  <!-- If this type has properties, display them -->
  <xsl:if test="//member[contains(@name,concat('P:',$FullMemberName))]">
    <h3>Properties</h3>
    <div class="contents">
      <xsl:for-each select="//member[contains(@name,concat('P:',$FullMemberName,'.'))]">
        <p class="name" id="{@name}">
          <xsl:value-of select="substring-after(@name, concat('P:',$FullMemberName,'.'))"/>
        </p>
        <xsl:apply-templates/>
      </xsl:for-each>
    </div>
  </xsl:if>
   
  <!-- If this type has methods, display them -->
  <xsl:if test="//member[contains(@name,concat('M:',$FullMemberName,'.'))]">
    <h3>Methods</h3>
    <div class="contents">
      <xsl:for-each select="//member[contains(@name,concat('M:',$FullMemberName,'.'))]">
        <!-- If this is a constructor, display the type name 
          (instead of "#ctor"), or display the method name -->
        <p class="name" id="{@name}">
          <xsl:choose>
            <xsl:when test="contains(@name, '#ctor')">
              Constructor:
              <xsl:value-of select="$MemberName"/>
              <xsl:value-of select="substring-after(@name, '#ctor')"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="substring-after(@name, concat('M:',$FullMemberName,'.'))"/>
            </xsl:otherwise>
          </xsl:choose>
        </p>

        <xsl:apply-templates select="summary"/>
        <xsl:apply-templates select="remarks"/>
        <xsl:apply-templates select="seealso"/>

        <!-- Display parameters if there are any -->
        <xsl:if test="count(param)!=0">
          <h4>Parameters</h4>
          <div class="contents">
            <xsl:apply-templates select="param"/>
          </div>
        </xsl:if>

        <!-- Display return value if there are any -->
        <xsl:if test="count(returns)!=0">
          <h4>Return Value</h4>
          <xsl:apply-templates select="returns"/>
        </xsl:if>

        <!-- Display exceptions if there are any -->
        <xsl:if test="count(exception)!=0">
          <h4>Exceptions</h4>
          <xsl:apply-templates select="exception"/>
        </xsl:if>

        <!-- Display examples if there are any -->
        <xsl:if test="count(example)!=0">
          <h4>Examples</h4>
          <xsl:apply-templates select="example"/>
        </xsl:if>

      </xsl:for-each>
    </div>
  </xsl:if>
  </div>
</xsl:template>

<!-- OTHER TEMPLATES -->

<!-- "paragraphs" - utility template for elements that can contain -->
<!-- either a single block of text or a sequence of <para> elements. -->
<xsl:template name="paragraphs">
  <xsl:choose>
    <xsl:when test="count(para)!=0">
      <xsl:apply-templates/>
    </xsl:when>
    <xsl:otherwise>
      <p>
        <xsl:apply-templates/>
      </p>
    </xsl:otherwise>
  </xsl:choose>
</xsl:template>

<!-- Templates for other tags -->
<xsl:template match="c">
  <code><xsl:apply-templates /></code>
</xsl:template>

<xsl:template match="code">
  <pre><xsl:apply-templates /></pre>
</xsl:template>

<xsl:template match="example">
  <xsl:call-template name="paragraphs"/>
</xsl:template>

<xsl:template match="exception">
  <p><span class="name"><xsl:value-of select="substring-after(@cref,'T:')"/>: </span><xsl:apply-templates /></p>
</xsl:template>

<xsl:template match="include">
  <a href="{@file}">External file</a>
</xsl:template>

<xsl:template match="para">
  <p><xsl:apply-templates /></p>
</xsl:template>

<xsl:template match="param">
  <p><span class="name"><xsl:value-of select="@name"/>: </span><xsl:apply-templates /></p>
</xsl:template>

<xsl:template match="paramref">
  <span class="ref"><xsl:value-of select="@name" /></span>
</xsl:template>

<xsl:template match="permission">
  <p><strong>Permission: </strong><span class="ref"><xsl:value-of select="@cref" /> </span><xsl:apply-templates /></p>
</xsl:template>

<xsl:template match="remarks">
  <xsl:call-template name="paragraphs"/>
</xsl:template>

<xsl:template match="returns">
  <xsl:call-template name="paragraphs"/>
</xsl:template>

<xsl:template match="see">
  <a href="#{@cref}"><xsl:value-of select="substring-after(@cref, ':')" /></a>
</xsl:template>

<xsl:template match="seealso">
  <p><a href="#{@cref}">See also: <xsl:value-of select="substring-after(@cref, ':')" /></a></p>
</xsl:template>

<xsl:template match="summary">
  <xsl:call-template name="paragraphs"/>
</xsl:template>

<xsl:template match="list">
  <xsl:choose>
    <xsl:when test="@type='bullet'">
      <ul>
      <xsl:for-each select="listheader">
        <li><strong><xsl:value-of select="term"/>: </strong><xsl:value-of select="definition"/></li>
      </xsl:for-each>
      <xsl:for-each select="list">
        <li><strong><xsl:value-of select="term"/>: </strong><xsl:value-of select="definition"/></li>
      </xsl:for-each>
      </ul>
    </xsl:when>
    <xsl:when test="@type='number'">
      <ol>
      <xsl:for-each select="listheader">
        <li><strong><xsl:value-of select="term"/>: </strong><xsl:value-of select="definition"/></li>
      </xsl:for-each>
      <xsl:for-each select="list">
        <li><strong><xsl:value-of select="term"/>: </strong><xsl:value-of select="definition"/></li>
      </xsl:for-each>
      </ol>
    </xsl:when>
    <xsl:when test="@type='table'">
      <table>
      <xsl:for-each select="listheader">
        <th>
          <td><xsl:value-of select="term"/></td>
          <td><xsl:value-of select="definition"/></td>
        </th>
      </xsl:for-each>
      <xsl:for-each select="list">
        <tr>
          <td><strong><xsl:value-of select="term"/>: </strong></td>
          <td><xsl:value-of select="definition"/></td>
        </tr>
      </xsl:for-each>
      </table>
    </xsl:when>
  </xsl:choose>
</xsl:template>

</xsl:stylesheet>
